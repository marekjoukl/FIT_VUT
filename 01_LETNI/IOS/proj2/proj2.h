/***********************************
 * 
 *  NAME:   IOS proj2 post
 *  AUTHOR: Marek Joukl (xjoukl00)
 *  DATE:   26.4.2023
 * 
***********************************/

#include <string.h>
#include <stdio.h>
#include <semaphore.h>
#include <fcntl.h>
#include <stdlib.h>
#include <sys/mman.h>
#include <sys/wait.h>
#include <unistd.h>
#include <stdbool.h>
#include <time.h>
#include <ctype.h>

#define FILENAME "proj2.out"
#define MMAP(pointer) {(pointer) = mmap(NULL, sizeof(*(pointer)), PROT_READ | PROT_WRITE, MAP_SHARED | MAP_ANONYMOUS, -1, 0);}
#define MUNMAP(pointer) {munmap((pointer), sizeof((pointer)));}
#define MOD 0666

/* File ptr */
FILE *file;

// Semaphores
sem_t *mutex, *queue1, *queue2, *queue3;

// Variables
unsigned int *A_count, *id_customer, *id_officer, *is_open, *queue1_count, *queue2_count, *queue3_count;

struct args {
    unsigned int NZ,    // Number of customers
                 NU,    // Number of officers
                 TZ,    // Max time (ms) that customer waits before entering post (or leave otherwise) 0<=TZ<=10000
                 TU,    // Max break time (ms) 0<=TU<=100
                 F;     // Max time (ms) that the post is closed
};

// Function that handles input from user
bool args_handle(int argc, char **argv, struct args *arguments);

// Function that checks wether arguments are in the right interval
bool input_check(int arr[]);

// Function that handles file opening
bool file_open(const char *filename);

// Initialize semaphores and variables
bool setup(void);

// Destruct semaphores and variables
void teardown(void);

// Handles customer functionality
void customer_proc(struct args arguments, unsigned int idZ);

// Handles officer functionality
void officer_proc(struct args arguments, unsigned int idU);

/********************** END OF FILE proj2.h **********************/
