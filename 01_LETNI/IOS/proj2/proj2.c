/***********************************
 * 
 *  NAME:   IOS proj2 post
 *  AUTHOR: Marek Joukl (xjoukl00)
 *  DATE:   26.4.2023
 * 
***********************************/

#include "proj2.h"

bool file_open(const char *filename){
    if ((file = fopen(filename, "w")) == NULL)
    {
        fprintf(stderr, "File could not be opened!\n");
        return false;
    }
    return true;
}

bool input_check(int arr[]) {
    if (arr[2] < 0 || arr[2] > 10000) {
        fprintf(stderr, "Number %d is in wrong interval! Expected number in interval <0,10000>\n",arr[2]);
        return false;
    } else if (arr[3] < 0 || arr[3] > 100)
    {
        fprintf(stderr, "Number %d is in wrong interval! Expected number in interval <0,100>\n",arr[3]);
        return false;
    } else if (arr[4] < 0 || arr[4] > 10000)
    {
        fprintf(stderr, "Number %d is in wrong interval! Expected number in interval <0,10000>\n",arr[4]);
        return false;
    }
    return true;
}

bool args_handle(int argc, char **argv, struct args *arguments){
    if (argc != 6)
    {   
        fprintf(stderr, "Wrong format of input. Expected 5 arguments! You wrote %d.\n",argc-1);
        return false;
    }
    // check if input is a number
    for (int i = 1; i < argc; i++) {
        int j;
        for (j = 0; argv[i][j] != '\0'; j++) {
            if (!isdigit(argv[i][j])) {
                printf("Argument %d is not a number\n", i);
                return false;
            }
        }
    }

    int values[5];
    for (int i = 1; i < 6; i++)
    {   
        {
            values[i-1] = atoi(argv[i]);    // store input into array as numbers      
        }
    }
    
    if (!input_check(values)) return false;

    // store input into global variable
    *arguments = (struct args) {
        .NZ = values[0],
        .NU = values[1],
        .TZ = values[2],
        .TU = values[3],
        .F = values[4]
    };
    return true;
}

bool setup(void){
    MMAP(A_count);
    MMAP(id_customer);
    MMAP(id_officer);
    MMAP(is_open);
    MMAP(queue1_count);
    MMAP(queue2_count);
    MMAP(queue3_count);
    *queue3_count = 0;
    *queue2_count = 0;
    *queue1_count = 0;
    *is_open = 1;
    *A_count = 0;
    *id_customer = 1;
    *id_officer = 1;

    if ((mutex = sem_open("/xjoukl00_ios_mutex", O_CREAT, MOD, 1)) == SEM_FAILED) return false;
    if ((queue1 = sem_open("/xjoukl00_ios_queue1", O_CREAT, MOD, 0)) == SEM_FAILED) return false;
    if ((queue2 = sem_open("/xjoukl00_ios_queue2", O_CREAT, MOD, 0)) == SEM_FAILED) return false;
    if ((queue3 = sem_open("/xjoukl00_ios_queue3", O_CREAT, MOD, 0)) == SEM_FAILED) return false;

    return true;
}

void teardown(void){
    MUNMAP(A_count);
    MUNMAP(id_customer);
    MUNMAP(id_officer);
    MUNMAP(queue1_count);
    MUNMAP(queue2_count);
    MUNMAP(queue3_count);
    MUNMAP(is_open);
    
    sem_close(mutex);
    sem_close(queue1);
    sem_close(queue2);
    sem_close(queue3);

    sem_unlink("/xjoukl00_ios_mutex");
    sem_unlink("/xjoukl00_ios_queue1");
    sem_unlink("/xjoukl00_ios_queue2");
    sem_unlink("/xjoukl00_ios_queue3");

    fclose(file);
}


void customer_proc(struct args arguments, unsigned int idZ){
    // start of a customer
    sem_wait(mutex);
        fprintf(file, "%d: Z %d: started\n", ++*A_count, idZ);
    sem_post(mutex);

    // customer sleeps for a given interval
    sem_wait(mutex);
        srand(time(NULL) * getpid());
        int sleep_time1 = (rand() % (arguments.TZ + 1)) * 1000;
    sem_post(mutex);
    usleep(sleep_time1);

    // if post is closed, customer leaves
    sem_wait(mutex);
    if (!*is_open){
            fprintf(file, "%d: Z %d: going home\n", ++*A_count, idZ);
        sem_post(mutex);
        exit(0);
    }

    // customer randomly selects a service <1,3> and joins the queue
    srand(time(NULL) * getpid());
    int service = (rand() % 3) + 1;
    fprintf(file, "%d: Z %d: entering office for a service %d\n",++*A_count, idZ, service);
    if(*is_open){    
        switch(service) {
            case 1:
                (*queue1_count)++;
                sem_post(mutex);
                sem_wait(queue1);
                break;
            case 2:
                (*queue2_count)++;
                sem_post(mutex);
                sem_wait(queue2);
                break;
            case 3:            
                (*queue3_count)++;
                sem_post(mutex);
                sem_wait(queue3);
                break;
        }
    } else{
        sem_post(mutex);
    }
    // after sync with the officer, the customer can proceed
    sem_wait(mutex);
        fprintf(file, "%d: Z %d: called by office worker\n", *A_count, idZ);
    sem_post(mutex);

    // customer sleeps random time in interval <0,10>
    sem_wait(mutex);
        srand(time(NULL) * getpid());
        int sleep_time2 = (rand() % 11) * 1000;
    sem_post(mutex);
    usleep(sleep_time2);

    // customer leaves
    sem_wait(mutex);
        fprintf(file, "%d: Z %d: going home\n", *A_count, idZ);
    sem_post(mutex);
}

void officer_proc(struct args arguments, unsigned int idU) {
    // start of a officer 
    sem_wait(mutex);
        fprintf(file, "%d: U %d: started\n", ++*A_count, idU);
    sem_post(mutex);
    
    while (true)
    {    
        // officer syncs with customer, decrements people in line and sleeps in interval <0,10>
        sem_wait(mutex);
        if (*queue1_count > 0) {
            fprintf(file, "%d: U %d: serving a service of type 1\n", ++*A_count, idU);
            (*queue1_count)--;
            sem_post(mutex);

            sem_post(queue1);

            sem_wait(mutex);
                srand(time(NULL) * getpid());
                int sleep_time3 = (rand() % 11) * 1000;
            sem_post(mutex);   
            usleep(sleep_time3);

            sem_wait(mutex);
                fprintf(file, "%d: U %d: service finished\n", ++*A_count, idU);
            sem_post(mutex);
        }
        else if (*queue2_count > 0) {
            fprintf(file, "%d: U %d: serving a service of type 2\n", ++*A_count, idU);
            (*queue2_count)--;
            sem_post(mutex);

            sem_post(queue2); 
            
            sem_wait(mutex);
                srand(time(NULL) * getpid());
                int sleep_time4 = (rand() % 11) * 1000;
            sem_post(mutex);
            usleep(sleep_time4);

            sem_wait(mutex);
                fprintf(file, "%d: U %d: service finished\n", ++*A_count, idU);
            sem_post(mutex);
        }

        else if (*queue3_count > 0) {
            fprintf(file, "%d: U %d: serving a service of type 3\n", ++*A_count, idU);
            (*queue3_count)--;
            sem_post(mutex);

            sem_post(queue3);
            
            sem_wait(mutex);
                srand(time(NULL) * getpid());
                int sleep_time5 = (rand() % 11) * 1000;
            sem_post(mutex);
            usleep(sleep_time5);

            sem_wait(mutex);
                fprintf(file, "%d: U %d: service finished\n", ++*A_count, idU);
            sem_post(mutex);
        } 
        else {
            // if there are no customers and the post is open, officer takes a break
            if (*is_open) {
                    fprintf(file, "%d: U %d: taking break\n", ++*A_count, idU);
                sem_post(mutex);
                
                sem_wait(mutex);
                    srand(time(NULL) * getpid());
                    int sleep_time6 = (rand() % (arguments.TU + 1)) * 1000;
                sem_post(mutex);
                usleep(sleep_time6);

                sem_wait(mutex);
                    fprintf(file, "%d: U %d: break finished\n", ++*A_count, idU);
                sem_post(mutex);
            }
            // if there are no customers and the post closed, process ends 
            else {
                sem_post(mutex);
                break;
            }
        }
    } // end of while loop
    //officer leaves
    sem_wait(mutex);
    fprintf(file, "%d: U %d: going home\n", ++*A_count, idU);
    sem_post(mutex);
}

int main (int argc, char **argv) {
    setbuf(stdout, NULL);
    if (!file_open(FILENAME)) return 1;

    struct args arguments;

    if(!args_handle(argc, argv, &arguments)) return 1;
    if(!setup()){
        teardown();
        fprintf(stderr, "Semaphore initialization was not succesful!");
        return 1;
    }

    // output buffering
    setbuf(file, NULL); 
    
    srand(time(NULL));

    for (unsigned int i = 1; i <= arguments.NZ; i++) {
        pid_t pid = fork();
            if (pid == 0) {
            // Child process (customer)
            customer_proc(arguments, (*id_customer)++);
            teardown();
            exit(0);
        }
    }

    for (unsigned int i = 1; i <= arguments.NU; i++) {
        pid_t pid = fork();
        if (pid == 0) {
            // Child process (officer)
            officer_proc(arguments, (*id_officer)++);
            teardown();
            exit(0);
        }
    }

    // main process waits in interval <F/2, F>
    sem_wait(mutex);
        srand(time(NULL)*getpid());
        int sleep_time = ((arguments.F / 2) + (rand() % ((arguments.F + 1))) / 2);
    sem_post(mutex);
    usleep(sleep_time * 1000);
    
    // post closing
    sem_wait(mutex);
        *is_open = 0;
        fprintf(file, "%d: closing\n", ++*A_count);
    sem_post(mutex);

    // waiting for end of all child processes
    while (wait(NULL) > 0);

    teardown();
    return 0;
}
/********************** END OF FILE proj2.c **********************/
