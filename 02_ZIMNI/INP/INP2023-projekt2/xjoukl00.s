; Autor reseni: Marek Joukl xjoukl00
; Pocet cyklu k serazeni puvodniho retezce: 3251
; Pocet cyklu razeni sestupne serazeneho retezce: 4049
; Pocet cyklu razeni vzestupne serazeneho retezce: 249
; Pocet cyklu razeni retezce s vasim loginem: 785
; Implementovany radici algoritmus: bubble-sort
; ------------------------------------------------

; DATA SEGMENT
                .data
; login:          .asciiz "vitejte-v-inp-2023"    ; puvodni uvitaci retezec
 login:          .asciiz "vvttpnjiiee3220---"  ; sestupne serazeny retezec
; login:          .asciiz "---0223eeiijnpttvv"  ; vzestupne serazeny retezec
; login:          .asciiz "xjoukl00"            ; SEM DOPLNTE VLASTNI LOGIN
                                                ; A POUZE S TIMTO ODEVZDEJTE

params_sys5:    .space  8   ; misto pro ulozeni adresy pocatku
                            ; retezce pro vypis pomoci syscall 5
                            ; (viz nize - "funkce" print_string)

; CODE SEGMENT
                .text
main:
        ; SEM DOPLNTE VASE RESENI


        daddi r4, r0, login             ; vozrovy vypis: adresa login: do r4
        jal count_symbols
        daddi   r14, r5, 0        
        syscall 1               ; systemova procedura - vypis cisla na terminal
        syscall 0  

count_symbols:  ; function to count symbols in a string
                daddi   r5, r0, 0      ; initialize counter to 0
count_loop:     lb      r6, 0(r4)      ; load byte at address in r4
                beqz    r6, count_end  ; if byte is zero (end of string), exit loop
                daddi   r5, r5, 1      ; increment counter
                daddi   r4, r4, 1      ; increment address to next byte
                j       count_loop     ; repeat loop
count_end:      ; r5 now holds the length of the string
                jr      r31            ; return to caller

                             ; halt
        
print_string:   ; adresa retezce se ocekava v r4
                sw      r4, params_sys5(r0)
                daddi   r14, r0, params_sys5    ; adr pro syscall 5 musi do r14
                syscall 5   ; systemova procedura - vypis retezce na terminal
                jr      r31 ; return - r31 je urcen na return address
