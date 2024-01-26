/**
 * Kostra programu pro 2. projekt IZP 2022/23
 *
 * Jednoducha shlukova analyza: 2D nejblizsi soused.
 * Single linkage
 * 
 *  - VUT FIT, 1BIT
 *  - IZP PROJEKT 2
 *  - Marek Joukl
 */
#include <stdio.h>
#include <stdlib.h>
#include <assert.h>
#include <math.h> // sqrtf
#include <limits.h> // INT_MAX

/*****************************************************************
 * Ladici makra. Vypnout jejich efekt lze definici makra
 * NDEBUG, napr.:
 *   a) pri prekladu argumentem prekladaci -DNDEBUG
 *   b) v souboru (na radek pred #include <assert.h>
 *      #define NDEBUG
 */
#ifdef NDEBUG
#define debug(s)
#define dfmt(s, ...)
#define dint(i)
#define dfloat(f)
#else

// vypise ladici retezec
#define debug(s) printf("- %s\n", s)

// vypise formatovany ladici vystup - pouziti podobne jako printf
#define dfmt(s, ...) printf(" - "__FILE__":%u: "s"\n",__LINE__,__VA_ARGS__)

// vypise ladici informaci o promenne - pouziti dint(identifikator_promenne)
#define dint(i) printf(" - " __FILE__ ":%u: " #i " = %d\n", __LINE__, i)

// vypise ladici informaci o promenne typu float - pouziti
// dfloat(identifikator_promenne)
#define dfloat(f) printf(" - " __FILE__ ":%u: " #f " = %g\n", __LINE__, f)

#endif

/*****************************************************************
 * Deklarace potrebnych datovych typu:
 *
 * TYTO DEKLARACE NEMENTE
 *
 *   struct obj_t - struktura objektu: identifikator a souradnice
 *   struct cluster_t - shluk objektu:
 *      pocet objektu ve shluku,
 *      kapacita shluku (pocet objektu, pro ktere je rezervovano
 *          misto v poli),
 *      ukazatel na pole shluku.
 */

struct obj_t {
    int id;
    float x;
    float y;
};

struct cluster_t {
    int size;
    int capacity;
    struct obj_t *obj;
};

/*****************************************************************
 * Deklarace potrebnych funkci.
 *
 * PROTOTYPY FUNKCI NEMENTE
 *
 * IMPLEMENTUJTE POUZE FUNKCE NA MISTECH OZNACENYCH 'TODO'
 *
 */

/*
 Inicializace shluku 'c'. Alokuje pamet pro cap objektu (kapacitu).
 Ukazatel NULL u pole objektu znamena kapacitu 0.
*/
void init_cluster(struct cluster_t *c, int cap)
{
    assert(c != NULL);
    assert(cap >= 0);

    c->obj = NULL;
    c->capacity = cap;
    c->size = 0;
    if (cap > 0){
        c->obj = (struct obj_t*)malloc(cap * sizeof(struct obj_t));
        if (c->obj == NULL)  c->capacity = 0;
    }
}

/*
 Odstraneni vsech objektu shluku a inicializace na prazdny shluk.
 */
void clear_cluster(struct cluster_t *c)
{
    free(c->obj);
    init_cluster(c, 0);
}
/*
Pomocna funkce pro odstraneni shluku.
*/
void clear_clusters(struct cluster_t *c, int count){
    for (int i = 0; i < count; i++)
    {
        clear_cluster(&c[i]);
    }
    free(c);
}

/// Chunk of cluster objects. Value recommended for reallocation.
const int CLUSTER_CHUNK = 10;

/*
 Zmena kapacity shluku 'c' na kapacitu 'new_cap'.
 */
struct cluster_t *resize_cluster(struct cluster_t *c, int new_cap)
{
    // TUTO FUNKCI NEMENTE
    assert(c);
    assert(c->capacity >= 0);
    assert(new_cap >= 0);

    if (c->capacity >= new_cap)
        return c;

    size_t size = sizeof(struct obj_t) * new_cap;

    void *arr = realloc(c->obj, size);
    if (arr == NULL)
        return NULL;

    c->obj = (struct obj_t*)arr;
    c->capacity = new_cap;
    return c;
}

/*
 Prida objekt 'obj' na konec shluku 'c'. Rozsiri shluk, pokud se do nej objekt
 nevejde.
 */
void append_cluster(struct cluster_t *c, struct obj_t obj)
{
    if (c->size >= c->capacity) resize_cluster(c, c->capacity + CLUSTER_CHUNK);

    c->obj[c->size] = obj;
    ++c->size;
}

/*
 Seradi objekty ve shluku 'c' vzestupne podle jejich identifikacniho cisla.
 */
void sort_cluster(struct cluster_t *c);

/*
 Do shluku 'c1' prida objekty 'c2'. Shluk 'c1' bude v pripade nutnosti rozsiren.
 Objekty ve shluku 'c1' budou serazeny vzestupne podle identifikacniho cisla.
 Shluk 'c2' bude nezmenen.
 */
void merge_clusters(struct cluster_t *c1, struct cluster_t *c2)
{
    assert(c1 != NULL);
    assert(c2 != NULL);

    for (int i = 0; i < c2->size; i++)
    {
        append_cluster(c1, c2->obj[i]);
    }
    sort_cluster(c1);
}

/**********************************************************************/
/* Prace s polem shluku */

/*
 Odstrani shluk z pole shluku 'carr'. Pole shluku obsahuje 'narr' polozek
 (shluku). Shluk pro odstraneni se nachazi na indexu 'idx'. Funkce vraci novy
 pocet shluku v poli.
*/
int remove_cluster(struct cluster_t *carr, int narr, int idx)
{
    assert(idx < narr);
    assert(narr > 0);

    clear_cluster(&carr[idx]);
    for (int i = idx; i < narr - 1; i++) carr[i] = carr[i+1];
    return narr - 1;
}

/*
 Pocita Euklidovskou vzdalenost mezi dvema objekty.
 */
float obj_distance(struct obj_t *o1, struct obj_t *o2)
{
    assert(o1 != NULL);
    assert(o2 != NULL);

    float x, y, dist;
    x = o2->x - o1->x;
    y = o2->y - o1->y;
    dist = sqrtf((x*x) + (y*y));
    return dist;
}

/*
 Pocita vzdalenost dvou shluku.
*/
float cluster_distance(struct cluster_t *c1, struct cluster_t *c2)
{
    assert(c1 != NULL);
    assert(c1->size > 0);
    assert(c2 != NULL);
    assert(c2->size > 0);

    float current_dist, smallest_dist = INT_MAX;
    for (int i = 0; i < c1->size; i++)
    {
        for (int j = 0; j < c2->size; j++)
        {
            current_dist = obj_distance(&c1->obj[i], &c2->obj[j]);
            if (current_dist < smallest_dist)
            {
                smallest_dist = current_dist;
            }
        }
    }
    return smallest_dist;
}

/*
 Funkce najde dva nejblizsi shluky. V poli shluku 'carr' o velikosti 'narr'
 hleda dva nejblizsi shluky. Nalezene shluky identifikuje jejich indexy v poli
 'carr'. Funkce nalezene shluky (indexy do pole 'carr') uklada do pameti na
 adresu 'c1' resp. 'c2'.
*/
void find_neighbours(struct cluster_t *carr, int narr, int *c1, int *c2)
{
    assert(narr > 0);

    float current_dist = 0.0, lowest_dist = INT_MAX;
    for (int i = 0; i < narr; i++)
    {
        for (int j = 0; j < narr; j++)
        {  
            if (i != j)
            {
                current_dist = cluster_distance(&carr[i], &carr[j]);
                if (current_dist < lowest_dist)
                {
                    lowest_dist = current_dist;
                    *c1 = i;
                    *c2 = j;
                }
            }
        }
    }
}

// pomocna funkce pro razeni shluku
static int obj_sort_compar(const void *a, const void *b)
{
    // TUTO FUNKCI NEMENTE
    const struct obj_t *o1 = (const struct obj_t *)a;
    const struct obj_t *o2 = (const struct obj_t *)b;
    if (o1->id < o2->id) return -1;
    if (o1->id > o2->id) return 1;
    return 0;
}

/*
 Razeni objektu ve shluku vzestupne podle jejich identifikatoru.
*/
void sort_cluster(struct cluster_t *c)
{
    // TUTO FUNKCI NEMENTE
    qsort(c->obj, c->size, sizeof(struct obj_t), &obj_sort_compar);
}

/*
 Tisk shluku 'c' na stdout.
*/
void print_cluster(struct cluster_t *c)
{
    // TUTO FUNKCI NEMENTE
    for (int i = 0; i < c->size; i++)
    {
        if (i) putchar(' ');
        printf("%d[%g,%g]", c->obj[i].id, c->obj[i].x, c->obj[i].y);
    }
    putchar('\n');
}

/*
 Ze souboru 'filename' nacte objekty. Pro kazdy objekt vytvori shluk a ulozi
 jej do pole shluku. Alokuje prostor pro pole vsech shluku a ukazatel na prvni
 polozku pole (ukalazatel na prvni shluk v alokovanem poli) ulozi do pameti,
 kam se odkazuje parametr 'arr'. Funkce vraci pocet nactenych objektu (shluku).
 V pripade nejake chyby uklada do pameti, kam se odkazuje 'arr', hodnotu NULL.
*/
int load_clusters(char *filename, struct cluster_t **arr)
{
    assert(arr != NULL);

    FILE *file = fopen(filename, "r");
    int lines, id, x, y, current_line = 1, error = 0;
    char c;

    if (file == NULL)
    {
        fprintf(stderr, "File could not be opened!\n");
        *arr = NULL;
        fclose(file);
        return 0;
    }

    fscanf(file, "count=%d", &lines);
    struct cluster_t *clusters = malloc(lines * sizeof(struct cluster_t));
    *arr = clusters;
    if (clusters == NULL)
    {
        free(clusters);
        *arr = NULL;
        fclose(file);
        return 0;
    }

    int arr_id[lines];
    while (lines >= current_line)
    {
        error = fscanf(file, "%d%d%d%c", &id, &x, &y, &c);
        arr_id[current_line - 1] = id; // check if id has been used before
        for (int i = 0; i < current_line; i++)
        {
            if (i < current_line - 1)
            {
                if (arr_id[i] == id)
                {
                    fprintf(stderr, "At least one object does not have unique id\n");
                    free(clusters);
                    *arr = NULL;
                    fclose(file);
                    return 0;
                }
            }
        }
        
        if (error == 4 || ( error == 3 && lines == current_line))
        {
            if (c == '\n' || c == EOF || c == '\r')
            {
                if (x > 1000 || x < 0 || y > 1000 || y < 0)
                {
                    fprintf(stderr, "Object coordinates must be between 0 and 1000!\n");
                    free(clusters);
                    *arr = NULL;
                    fclose(file);
                    return 0;
                }
                struct cluster_t cluster;
                init_cluster(&cluster, 1);
                cluster.size = 1;
                clusters[current_line - 1] = cluster;
                clusters[current_line - 1].obj->id = id;
                clusters[current_line - 1].obj->x = x;
                clusters[current_line - 1].obj->y = y;
                current_line++;
            } else {
                fprintf(stderr, "Wrong format of object informations\n");
                free(clusters);
                *arr = NULL;
                fclose(file);
                return 0;
            }
        } else {
            free(clusters);
            fprintf(stderr, "Wrong file format");
            *arr = NULL;
            fclose(file);
            return 0;
        }
    }
    fclose(file);
    return lines;
}

/*
 Tisk pole shluku. Parametr 'carr' je ukazatel na prvni polozku (shluk).
 Tiskne se prvnich 'narr' shluku.
*/
void print_clusters(struct cluster_t *carr, int narr)
{
    printf("Clusters:\n");
    for (int i = 0; i < narr; i++)
    {
        printf("cluster %d: ", i);
        print_cluster(&carr[i]);
    }
}

int main(int argc, char *argv[])
{
    char *filename = argv[1];
    int n = 1;
    
    if (argc == 3 || argc == 2){
        if (argc == 3) {
            //check if the coordinate is not a float
            if (atoi(argv[2]) <= 0 || atof(argv[2]) != atoi(argv[2])) {
                fprintf(stderr, "N must be integer bigger than 0\n");
                return -1;
            } else{
                n = atoi(argv[2]);
            }
        }
        
        struct cluster_t *clusters;
        int total_lines = load_clusters(filename, &clusters);
        
        if (!clusters)
        {
            clear_clusters(clusters, total_lines);
            return -1;
        } else if (n > total_lines)
        {
            fprintf(stderr, "The value of N cannot be higher than number of objects\n");
            clear_clusters(clusters, total_lines);
            return -1;
        }
        
        while (total_lines != n)
        {
            int c1, c2;
            find_neighbours(clusters, total_lines, &c1, &c2);
            merge_clusters(&clusters[c1], &clusters[c2]);
            total_lines = remove_cluster(clusters, total_lines, c2);
        }

        print_clusters(clusters, total_lines);
        clear_clusters(clusters, total_lines);
    } else {
        fprintf(stderr, "Wrong format of arguments\n");
        return -1;
    }
    return 0;
}