# ITS Projekt 1

- **Autor:** Marek Joukl (xjoukl00)
- **Datum:** 2024-04-03

## Matice pokrytí artefaktů

Čísla testů jednoznačně identifikují scénář v souborech `.feature`.

| Page                      | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10 | 11 | 12 | 13 | 14 | 15 | 16 |
|---------------------------|---|---|---|---|---|---|---|---|---|----|----|----|----|----|----|----|
| Home                      | x | x | x |   |   |   |   |   |   |    |    |    |    |    |    |    |
| Search                    | x | x |   |   |   |   |   |   |   |    |    |    |    |    |    |    |
| Results                   | x | x | x | x |   |   |   |   |   |    |    |    |    |    |    |    |
| Categories                |   |   | x |   |   |   |   |   |   |    |    |    |    |    |    |    |
| Product Detail            |   |   |   | x | x |   | x | x |   | x  |    |    |    |    |    |    |
| Shopping Cart             |   |   |   |   |   | x | x | x | x |    |    |    |    |    |    |    |
| Checkout                  |   |   |   |   |   |   |   |   | x | x  | x  |    |    |    |    |    |
| Confirmation              |   |   |   |   |   |   |   |   |   |    | x  |    |    |    |    |    |
| Dashbord (admin)          |   |   |   |   |   |   |   |   |   |    |    | x  |    |    |    |    |
| Products (admin)          |   |   |   |   |   |   |   |   |   |    |    | x  | x  | x  | x  | x  |
| Product details (admin)   |   |   |   |   |   |   |   |   |   |    |    |    | x  | x  |    |    |

## Matice pokrytí aktivit

| Activities                | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10 | 11 | 12 | 13 | 14 | 15 | 16 |
|---------------------------|---|---|---|---|---|---|---|---|---|----|----|----|----|----|----|----|
| Searching products        | x | x | x | x |   |   |   |   |   |    |    |    |    |    |    |    |
| Using shopping cart       |   |   |   |   | x | x | x | x |   |    |    |    |    |    |    |    |
| Proceeding to checkout    |   |   |   |   |   |   |   |   | x | x  |    |    |    |    |    |    |
| Placing order             |   |   |   |   |   |   |   |   |   |    | x  |    |    |    |    |    |
| Managing goods (admin)    |   |   |   |   |   |   |   |   |   |    |    | x  | x  | x  | x  | x  |


## Matice Feature-Test

| Feature file                  | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10 | 11 | 12 | 13 | 14 | 15 | 16 |
|-------------------------------|---|---|---|---|---|---|---|---|---|----|----|----|----|----|----|----|
| search_and_buy.feature        | x | x | x | x | x | x |   |   |   |    |    |    |    |    |    |    |
| checkout_and_purchase.feature |   |   |   |   |   |   | x | x | x |  x | x  |    |    |    |    |    |
| goods_management.feature      |   |   |   |   |   |   |   |   |   |    |    | x  | x  | x  | x  | x  |

