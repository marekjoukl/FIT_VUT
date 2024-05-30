-- IDS Projekt 2024 (2. cast)
-- Autori: xjoukl00, xkozan01
-- Datum: 21. 3. 2024

-- Drop
DROP TABLE Kontrola;
DROP TABLE Dohlizi_Na;
DROP TABLE Ucastni_Se_Alkoholik;
DROP TABLE Ucastni_Se_Odbornik;
DROP TABLE Ucastni_Se_Patron;
DROP TABLE Schuze;
DROP TABLE Misto;
DROP TABLE Odbornik;
DROP TABLE Patron;
DROP TABLE Alkoholik;
DROP MATERIALIZED VIEW XKOZAN01.Average_Alcohol_Level;
DROP INDEX idx_Mira_Alkoholu;
-- Create

-- Vztah generalizace/specializace byl vyresen vytvorenim tabulek pouze pro podtypy i s
-- atributy nadtypu (2. moznost z prezentace). Tento zpusob byl zvolen z duvodu, ze
-- je specializace totalni. Je bohuzel i disjunktni, a tak se zde objevi castecna redundance
-- dat.

CREATE TABLE Alkoholik (
    ID_Osoby INTEGER GENERATED AS IDENTITY PRIMARY KEY,
    Jmeno VARCHAR(50) NOT NULL,
    Prijmeni VARCHAR(50) NOT NULL,
    Pohlavi VARCHAR(10) NOT NULL CHECK (Pohlavi IN ('Muž', 'Žena', 'Jine')),
    Tel_cislo VARCHAR(16) NOT NULL,
    Email VARCHAR(50),
    Adresa VARCHAR(50) NOT NULL,
    Datum_Posledniho_Uziti DATE
);

CREATE TABLE Odbornik (
                          ID_Osoby INTEGER GENERATED AS IDENTITY PRIMARY KEY,
                          Jmeno VARCHAR(50) NOT NULL,
                          Prijmeni VARCHAR(50) NOT NULL,
                          Pohlavi VARCHAR(10) NOT NULL CHECK (Pohlavi IN ('Muž', 'Žena', 'Jine')),
                          Tel_cislo VARCHAR(16) NOT NULL,
                          Email VARCHAR(50),
                          Adresa VARCHAR(50) NOT NULL,
                          Expertiza VARCHAR(25) NOT NULL,
                          Praxe VARCHAR(25) NOT NULL
);


CREATE TABLE Patron (
   ID_Osoby INTEGER GENERATED AS IDENTITY PRIMARY KEY,
    Jmeno VARCHAR(50) NOT NULL,
    Prijmeni VARCHAR(50) NOT NULL,
    Pohlavi VARCHAR(10) NOT NULL CHECK (Pohlavi IN ('Muž', 'Žena', 'Jine')),
    Tel_cislo VARCHAR(16) NOT NULL,
    Email VARCHAR(50),
    Adresa VARCHAR(50) NOT NULL,
    Zacatek_Patronstvi DATE,
    Sverenec INT NOT NULL,

    CONSTRAINT FK_Podporuje
                    FOREIGN KEY (Sverenec)
                    REFERENCES Alkoholik(ID_Osoby)
                    ON DELETE CASCADE
);

CREATE TABLE Misto (
    ID_mista INTEGER GENERATED AS IDENTITY PRIMARY KEY,
    Nazev VARCHAR(50) NOT NULL,
    Lokalita VARCHAR(50) NOT NULL,
    Kapacita INTEGER NOT NULL
);

CREATE TABLE Schuze (
    ID_Schuze INTEGER GENERATED AS IDENTITY PRIMARY KEY,
    datum DATE,
    cas TIMESTAMP,
    Poradove_Cislo INTEGER NOT NULL CHECK ( Poradove_Cislo > 0 ),
    Vedouci_Alkoholik INTEGER,
    Vedouci_Odbornik INTEGER,
    Vedouci_Patron INTEGER,
    Kona_se INTEGER NOT NULL,

    CONSTRAINT FK_Misto
        FOREIGN KEY (Kona_se)
        REFERENCES Misto(ID_mista)
        ON DELETE CASCADE,

    CONSTRAINT FK_Schuze_Vedouci_Alkoholik
        FOREIGN KEY (Vedouci_Alkoholik)
        REFERENCES Alkoholik(ID_Osoby)
        ON DELETE CASCADE,

    CONSTRAINT FK_Schuze_Vedouci_Odbornik
        FOREIGN KEY (Vedouci_Odbornik)
        REFERENCES Odbornik(ID_Osoby)
        ON DELETE CASCADE,

    CONSTRAINT FK_Schuze_Vedouci_Patron
        FOREIGN KEY (Vedouci_Patron)
        REFERENCES Patron(ID_Osoby)
        ON DELETE CASCADE,

    CONSTRAINT Only_One_Vedouci
        CHECK (
            (Vedouci_Alkoholik IS NOT NULL AND Vedouci_Odbornik IS NULL AND Vedouci_Patron IS NULL) OR
            (Vedouci_Alkoholik IS NULL AND Vedouci_Odbornik IS NOT NULL AND Vedouci_Patron IS NULL) OR
            (Vedouci_Alkoholik IS NULL AND Vedouci_Odbornik IS NULL AND Vedouci_Patron IS NOT NULL)
        )
);



CREATE TABLE Kontrola (
    Poradove_Cislo INTEGER GENERATED AS IDENTITY NOT NULL ,
    Mira_Alkoholu FLOAT NOT NULL CHECK (Mira_Alkoholu >= 0 AND Mira_Alkoholu <= 10),
    Puvod_Alkoholu VARCHAR(50) NOT NULL ,
    Typ_Alkoholu VARCHAR(25) NOT NULL ,
    Testovany_Alkoholik INTEGER NOT NULL ,
    Vykonava_Odbornik INTEGER NOT NULL ,

    CONSTRAINT PK_Kontrola
                      PRIMARY KEY (Poradove_Cislo, Testovany_Alkoholik),
    CONSTRAINT FK_Kontrola_Alkoholik
                      FOREIGN KEY (Testovany_Alkoholik)
                      REFERENCES Alkoholik(ID_Osoby)
                      ON DELETE CASCADE,
    CONSTRAINT FK_Kontrola_Odbornik
                      FOREIGN KEY (Vykonava_Odbornik)
                      REFERENCES Odbornik(ID_Osoby)
                      ON DELETE CASCADE
);

CREATE TABLE Dohlizi_Na (
    Odbornik INTEGER NOT NULL ,
    Alkoholik INTEGER NOT NULL ,

    CONSTRAINT PK_Dohled
                        PRIMARY KEY (Odbornik, Alkoholik),
    CONSTRAINT FK_Dohled_Alkoholik
                        FOREIGN KEY (Alkoholik)
                        REFERENCES Alkoholik(ID_Osoby)
                        ON DELETE CASCADE,
    CONSTRAINT FK_Dohled_Odbornik
                        FOREIGN KEY (Odbornik)
                        REFERENCES Odbornik(ID_Osoby)
                        ON DELETE CASCADE
);

CREATE TABLE Ucastni_Se_Alkoholik(
    Alkoholik INTEGER NOT NULL ,
    Schuze INTEGER NOT NULL ,

    CONSTRAINT PK_Ucastnik_Schuze_Alkoholik
                        PRIMARY KEY (Alkoholik,Schuze),
    CONSTRAINT FK_Ucastnik_Schuze_Alkoholik
                        FOREIGN KEY (Schuze)
                        REFERENCES Schuze(ID_Schuze)
                        ON DELETE CASCADE,
    CONSTRAINT PK_Ucastnik_Alkoholik
                        FOREIGN KEY (Alkoholik)
                        REFERENCES Alkoholik(ID_Osoby)
                        ON DELETE CASCADE
);

CREATE TABLE Ucastni_Se_Odbornik(
    Odbornik INTEGER NOT NULL ,
    Schuze INTEGER NOT NULL ,

    CONSTRAINT PK_Ucastnik_Schuze_Odbornik
                        PRIMARY KEY (Odbornik,Schuze),
    CONSTRAINT FK_Ucastnik_Schuze_Odbornik
                        FOREIGN KEY (Schuze)
                        REFERENCES Schuze(ID_Schuze)
                        ON DELETE CASCADE,
    CONSTRAINT PK_Ucastnik_Odbornik
                        FOREIGN KEY (Odbornik)
                        REFERENCES Odbornik(ID_Osoby)
                        ON DELETE CASCADE
);

CREATE TABLE Ucastni_Se_Patron(
    Patron INTEGER NOT NULL ,
    Schuze INTEGER NOT NULL ,

    CONSTRAINT PK_Ucastnik_Schuze_Patron
                        PRIMARY KEY (Patron,Schuze),
    CONSTRAINT FK_Ucastnik_Schuze_Patron
                        FOREIGN KEY (Schuze)
                        REFERENCES Schuze(ID_Schuze)
                        ON DELETE CASCADE,
    CONSTRAINT PK_Ucastnik_Patron
                        FOREIGN KEY (Patron)
                        REFERENCES Patron(ID_Osoby)
                        ON DELETE CASCADE
);

-- NETRIVIALNI TRIGGERY --

-- kontrola maximalniho poctu alkoholiku na schuzi
CREATE OR REPLACE TRIGGER Check_Alcoholic_Count
BEFORE INSERT ON Ucastni_Se_Alkoholik
FOR EACH ROW
DECLARE
    Alcoholic_Count NUMBER;
BEGIN
    SELECT COUNT(*)
    INTO Alcoholic_Count
    FROM Ucastni_Se_Alkoholik
    WHERE Schuze = :NEW.Schuze;

    IF Alcoholic_Count >= 12 THEN
        RAISE_APPLICATION_ERROR(-20001, 'Maximalne 12 alkoholiku se muze ucastnit 1 schuze.');
    END IF;
END;
/

INSERT INTO Alkoholik (Jmeno, Prijmeni, Pohlavi, Tel_cislo, Email, Adresa, Datum_Posledniho_Uziti)
VALUES ('Alice', 'Johnson', 'Žena', '111222333', 'alice.johnson@example.com', '789 Oak Street', TO_DATE('01-JAN-23', 'DD-MON-YY'));
INSERT INTO Alkoholik (Jmeno, Prijmeni, Pohlavi, Tel_cislo, Email, Adresa, Datum_Posledniho_Uziti)
VALUES ('Bob', 'Williams', 'Muž', '444555666', 'bob.williams@example.com', '101 Pine Street', TO_DATE('15-FEB-23', 'DD-MON-YY'));
INSERT INTO Alkoholik (Jmeno, Prijmeni, Pohlavi, Tel_cislo, Email, Adresa, Datum_Posledniho_Uziti)
VALUES ('Alice', 'Johnson', 'Žena', '111222333', 'alice.johnson@example.com', '789 Oak Street', TO_DATE('01-JAN-23', 'DD-MON-YY'));
INSERT INTO Alkoholik (Jmeno, Prijmeni, Pohlavi, Tel_cislo, Email, Adresa, Datum_Posledniho_Uziti)
VALUES ('Bob', 'Williams', 'Muž', '444555666', 'bob.williams@example.com', '101 Pine Street', TO_DATE('15-FEB-23', 'DD-MON-YY'));
INSERT INTO Alkoholik (Jmeno, Prijmeni, Pohlavi, Tel_cislo, Email, Adresa, Datum_Posledniho_Uziti)
VALUES ('Alice', 'Johnson', 'Žena', '111222333', 'alice.johnson@example.com', '789 Oak Street', TO_DATE('01-JAN-23', 'DD-MON-YY'));
INSERT INTO Alkoholik (Jmeno, Prijmeni, Pohlavi, Tel_cislo, Email, Adresa, Datum_Posledniho_Uziti)
VALUES ('Bob', 'Williams', 'Muž', '444555666', 'bob.williams@example.com', '101 Pine Street', TO_DATE('15-FEB-23', 'DD-MON-YY'));
INSERT INTO Alkoholik (Jmeno, Prijmeni, Pohlavi, Tel_cislo, Email, Adresa, Datum_Posledniho_Uziti)
VALUES ('Alice', 'Johnson', 'Žena', '111222333', 'alice.johnson@example.com', '789 Oak Street', TO_DATE('01-JAN-23', 'DD-MON-YY'));
INSERT INTO Alkoholik (Jmeno, Prijmeni, Pohlavi, Tel_cislo, Email, Adresa, Datum_Posledniho_Uziti)
VALUES ('Bob', 'Williams', 'Muž', '444555666', 'bob.williams@example.com', '101 Pine Street', TO_DATE('15-FEB-23', 'DD-MON-YY'));
INSERT INTO Alkoholik (Jmeno, Prijmeni, Pohlavi, Tel_cislo, Email, Adresa, Datum_Posledniho_Uziti)
VALUES ('Alice', 'Johnson', 'Žena', '111222333', 'alice.johnson@example.com', '789 Oak Street', TO_DATE('01-JAN-23', 'DD-MON-YY'));
INSERT INTO Alkoholik (Jmeno, Prijmeni, Pohlavi, Tel_cislo, Email, Adresa, Datum_Posledniho_Uziti)
VALUES ('Bob', 'Williams', 'Muž', '444555666', 'bob.williams@example.com', '101 Pine Street', TO_DATE('15-FEB-23', 'DD-MON-YY'));
INSERT INTO Alkoholik (Jmeno, Prijmeni, Pohlavi, Tel_cislo, Email, Adresa, Datum_Posledniho_Uziti)
VALUES ('Alice', 'Johnson', 'Žena', '111222333', 'alice.johnson@example.com', '789 Oak Street', TO_DATE('01-JAN-23', 'DD-MON-YY'));
INSERT INTO Alkoholik (Jmeno, Prijmeni, Pohlavi, Tel_cislo, Email, Adresa, Datum_Posledniho_Uziti)
VALUES ('Alice', 'Johnson', 'Žena', '111222333', 'alice.johnson@example.com', '789 Oak Street', TO_DATE('01-JAN-23', 'DD-MON-YY'));

INSERT INTO Odbornik (Jmeno, Prijmeni, Pohlavi, Tel_cislo, Email, Adresa, Expertiza, Praxe)
VALUES ('Jane', 'Smith', 'Žena', '987654321', 'jane.smith@example.com', '456 Elm Street', 'Engineering', '10 years');
INSERT INTO Misto (Nazev, Lokalita, Kapacita) VALUES ('Konferencni mistnost', 'Praha', 50);
INSERT INTO Schuze (datum, cas, Poradove_Cislo, Vedouci_Alkoholik, Vedouci_Odbornik, Vedouci_Patron, Kona_se)
VALUES (TO_DATE('01-JAN-24', 'DD-MON-YY'), CURRENT_TIMESTAMP, 1, NULL, 1, NULL, 1);

INSERT INTO Ucastni_Se_Alkoholik (Alkoholik, Schuze)
VALUES (1, 1);

INSERT INTO Ucastni_Se_Alkoholik (Alkoholik, Schuze)
VALUES (2, 1);

INSERT INTO Ucastni_Se_Alkoholik (Alkoholik, Schuze)
VALUES (3, 1);

INSERT INTO Ucastni_Se_Alkoholik (Alkoholik, Schuze)
VALUES (4, 1);

INSERT INTO Ucastni_Se_Alkoholik (Alkoholik, Schuze)
VALUES (5, 1);

INSERT INTO Ucastni_Se_Alkoholik (Alkoholik, Schuze)
VALUES (6, 1);

INSERT INTO Ucastni_Se_Alkoholik (Alkoholik, Schuze)
VALUES (7, 1);

INSERT INTO Ucastni_Se_Alkoholik (Alkoholik, Schuze)
VALUES (8, 1);

INSERT INTO Ucastni_Se_Alkoholik (Alkoholik, Schuze)
VALUES (9, 1);

INSERT INTO Ucastni_Se_Alkoholik (Alkoholik, Schuze)
VALUES (10, 1);

INSERT INTO Ucastni_Se_Alkoholik (Alkoholik, Schuze)
VALUES (11, 1);

INSERT INTO Ucastni_Se_Alkoholik (Alkoholik, Schuze)
VALUES (12, 1);

-- zde se trigger spusti a zakaze 13 pridani alkoholika
INSERT INTO Ucastni_Se_Alkoholik (Alkoholik, Schuze)
VALUES (13, 1);


-- aktualizace datumu posledniho uziti alkoholu
CREATE OR REPLACE TRIGGER Update_Last_Usage_Date
BEFORE INSERT ON Kontrola
FOR EACH ROW
BEGIN
    IF :NEW.Mira_Alkoholu > 0 THEN
        UPDATE Alkoholik
        SET Datum_Posledniho_Uziti = SYSDATE
        WHERE ID_Osoby = :NEW.Testovany_Alkoholik;
    END IF;
END;
/

 ------ TEST 2. TRIGGERU ------
 -- vlozeni alkoholika
INSERT INTO Alkoholik (Jmeno, Prijmeni, Pohlavi, Tel_cislo, Email, Adresa, Datum_Posledniho_Uziti)
VALUES ('Alice', 'Johnson', 'Žena', '111222333', 'alice.johnson@example.com', '789 Oak Street', TO_DATE('01-JAN-23', 'DD-MON-YY'));

-- puvodni posedni datum uziti
SELECT Datum_Posledniho_Uziti FROM Alkoholik;

-- vlozeni odbornika
INSERT INTO Odbornik (Jmeno, Prijmeni, Pohlavi, Tel_cislo, Email, Adresa, Expertiza, Praxe)
VALUES ('John', 'Doe', 'Muž', '123456789', 'john.doe@example.com', '123 Main Street', 'Computer Science', '5 years');
-- vlozeni nove kontroly
INSERT INTO Kontrola (Mira_Alkoholu, Puvod_Alkoholu, Typ_Alkoholu, Testovany_Alkoholik, Vykonava_Odbornik)
VALUES (0.5, 'Czech', 'Pivo', 1, 1);

-- aktualizovane posedni datum uziti
SELECT Datum_Posledniho_Uziti FROM Alkoholik;

-- TEST VLOZENI DAT --
-- Example data for Odbornik table
INSERT INTO Odbornik (Jmeno, Prijmeni, Pohlavi, Tel_cislo, Email, Adresa, Expertiza, Praxe)
VALUES ('John', 'Doe', 'Muž', '123456789', 'john.doe@example.com', '123 Main Street', 'Computer Science', '5 years');

INSERT INTO Odbornik (Jmeno, Prijmeni, Pohlavi, Tel_cislo, Email, Adresa, Expertiza, Praxe)
VALUES ('Jane', 'Smith', 'Žena', '987654321', 'jane.smith@example.com', '456 Elm Street', 'Engineering', '10 years');

-- Example data for Alkoholik table
INSERT INTO Alkoholik (Jmeno, Prijmeni, Pohlavi, Tel_cislo, Email, Adresa, Datum_Posledniho_Uziti)
VALUES ('Alice', 'Johnson', 'Žena', '111222333', 'alice.johnson@example.com', '789 Oak Street', TO_DATE('01-JAN-23', 'DD-MON-YY'));

INSERT INTO Alkoholik (Jmeno, Prijmeni, Pohlavi, Tel_cislo, Email, Adresa, Datum_Posledniho_Uziti)
VALUES ('Bob', 'Williams', 'Muž', '444555666', 'bob.williams@example.com', '101 Pine Street', TO_DATE('15-FEB-23', 'DD-MON-YY'));

-- Example data for Patron table
INSERT INTO Patron (Jmeno, Prijmeni, Pohlavi, Tel_cislo, Email, Adresa, Zacatek_Patronstvi, Sverenec)
VALUES ('Michael', 'Brown', 'Muž', '777888999', 'michael.brown@example.com', '222 Maple Street', TO_DATE('01-JAN-24', 'DD-MON-YY'), 1);

INSERT INTO Patron (Jmeno, Prijmeni, Pohlavi, Tel_cislo, Email, Adresa, Zacatek_Patronstvi, Sverenec)
VALUES ('Emma', 'Davis', 'Žena', '123123123', 'emma.davis@example.com', '333 Birch Street', TO_DATE('15-JAN-24', 'DD-MON-YY'), 2);

-- Example data for Misto table
INSERT INTO Misto (Nazev, Lokalita, Kapacita) VALUES ('Konferencni mistnost', 'Praha', 50);
INSERT INTO Misto (Nazev, Lokalita, Kapacita) VALUES ('Meeting room A', 'New York', 30);
INSERT INTO Misto (Nazev, Lokalita, Kapacita) VALUES ('Sala de reuniones', 'Madrid', 40);

-- Example data for Schuze table
INSERT INTO Schuze (datum, cas, Poradove_Cislo, Vedouci_Alkoholik, Vedouci_Odbornik, Vedouci_Patron, Kona_se)
VALUES (TO_DATE('01-JAN-24', 'DD-MON-YY'), CURRENT_TIMESTAMP, 1, NULL, 2, NULL, 1);

INSERT INTO Schuze (datum, cas, Poradove_Cislo, Vedouci_Alkoholik, Vedouci_Odbornik, Vedouci_Patron, Kona_se)
VALUES (TO_DATE('15-FEB-24', 'DD-MON-YY'), CURRENT_TIMESTAMP, 2, NULL, NULL, 2, 2);

INSERT INTO Schuze (datum, cas, Poradove_Cislo, Vedouci_Alkoholik, Vedouci_Odbornik, Vedouci_Patron, Kona_se)
VALUES (TO_DATE('01-MAR-24', 'DD-MON-YY'), CURRENT_TIMESTAMP, 3, NULL, 1, NULL, 1);

-- Example data for Kontrola table
INSERT INTO Kontrola (Mira_Alkoholu, Puvod_Alkoholu, Typ_Alkoholu, Testovany_Alkoholik, Vykonava_Odbornik)
VALUES (0.5, 'Czech', 'Pivo', 1, 1);
INSERT INTO Kontrola (Mira_Alkoholu, Puvod_Alkoholu, Typ_Alkoholu, Testovany_Alkoholik, Vykonava_Odbornik)
VALUES (0.7, 'Czech', 'Pivo', 1, 1);
INSERT INTO Kontrola (Mira_Alkoholu, Puvod_Alkoholu, Typ_Alkoholu, Testovany_Alkoholik, Vykonava_Odbornik)
VALUES (0.9, 'Czech', 'Pivo', 1, 1);
INSERT INTO Kontrola (Mira_Alkoholu, Puvod_Alkoholu, Typ_Alkoholu, Testovany_Alkoholik, Vykonava_Odbornik)
VALUES (1.1, 'Russia', 'Vodka', 1, 1);

INSERT INTO Kontrola (Mira_Alkoholu, Puvod_Alkoholu, Typ_Alkoholu, Testovany_Alkoholik, Vykonava_Odbornik)
VALUES (1.2, 'Italy', 'Víno', 2, 2);

INSERT INTO Kontrola (Mira_Alkoholu, Puvod_Alkoholu, Typ_Alkoholu, Testovany_Alkoholik, Vykonava_Odbornik)
VALUES (1.4, 'Belgium', 'Pivo', 2, 2);

INSERT INTO Kontrola (Mira_Alkoholu, Puvod_Alkoholu, Typ_Alkoholu, Testovany_Alkoholik, Vykonava_Odbornik)
VALUES (1.5, 'Ukraine', 'Vodka', 2, 2);

-- Example data for Dohlizi_Na table
INSERT INTO Dohlizi_Na (Odbornik, Alkoholik)
VALUES (1, 1);

INSERT INTO Dohlizi_Na (Odbornik, Alkoholik)
VALUES (2, 2);

-- Example data for Ucastni_Se_Alkoholik table
INSERT INTO Ucastni_Se_Alkoholik (Alkoholik, Schuze)
VALUES (1, 3);

INSERT INTO Ucastni_Se_Alkoholik (Alkoholik, Schuze)
VALUES (2, 3);

-- Example data for Ucastni_Se_Odbornik table
INSERT INTO Ucastni_Se_Odbornik (Odbornik, Schuze)
VALUES (1, 1);

INSERT INTO Ucastni_Se_Odbornik (Odbornik, Schuze)
VALUES (2, 2);

-- Example data for Ucastni_Se_Patron table
INSERT INTO Ucastni_Se_Patron (Patron, Schuze)
VALUES (1, 1);

INSERT INTO Ucastni_Se_Patron (Patron, Schuze)
VALUES (2, 2);

-- SELECT PROMPTS --

-- Zobrazení všech účastníků schůzky ze všech tří tabulek s ID Schuze = 2 (spojeni 2 tabulek)
SELECT a.Jmeno, a.Prijmeni
FROM Alkoholik a
JOIN Ucastni_Se_Alkoholik u ON a.ID_Osoby = u.Alkoholik
WHERE u.Schuze = 2
UNION ALL
SELECT o.Jmeno, o.Prijmeni
FROM Odbornik o
JOIN Ucastni_Se_Odbornik u ON o.ID_Osoby = u.Odbornik
WHERE u.Schuze = 2
UNION ALL
SELECT p.Jmeno, p.Prijmeni
FROM Patron p
JOIN Ucastni_Se_Patron u ON p.ID_Osoby = u.Patron
WHERE u.Schuze = 2;

-- Zobrazeni vsech zaznamu alkoholiku jejichz mira namereneho alkoholu byla
-- vyssi nez 1 promile (sppjeni 2 tabulek) a serazeno sestupne
SELECT a.Jmeno, a.Prijmeni, Mira_Alkoholu
FROM Alkoholik a JOIN Kontrola k ON a.ID_Osoby = k.Testovany_Alkoholik
WHERE Mira_Alkoholu > 1
ORDER BY Mira_Alkoholu DESC;

-- Zobrazení všech alkoholiků, kterým byla provedena kontrola odborníkem John Doe a
-- typ alkoholu byl Pivo (spojeni 3 tabulek)
SELECT DISTINCT a.Jmeno, a.Prijmeni
FROM Alkoholik a
JOIN Kontrola k ON a.ID_Osoby = k.Testovany_Alkoholik
JOIN Odbornik o ON o.ID_Osoby = k.Vykonava_Odbornik
WHERE o.Jmeno = 'John' AND o.Prijmeni = 'Doe' AND k.Typ_Alkoholu = 'Pivo';

-- Zobrazeni poctu kontrol a prumerne namerene hodnoty miry
-- pro typ alkoholu Pivo u vsech alkoholiku (pouziti GROUP BY)
SELECT a.Jmeno,a.Prijmeni, COUNT(*) AS Pocet_Kontroly, AVG(k.Mira_Alkoholu) AS Prumerna_Mira_Alkoholu
FROM Alkoholik a
JOIN Kontrola k ON a.ID_Osoby = k.Testovany_Alkoholik
WHERE k.Typ_Alkoholu = 'Pivo'
GROUP BY a.Jmeno, a.Prijmeni;

-- Zobrazeni vsech schuzi, ktere se konaly bez ucasti alkoholiku. (pouziti EXISTS)
-- V systemu to muze znamenat, ze na schuzi se neprihlasil zadny alkoholik a muze byt zrusena.
SELECT s.ID_Schuze, s.datum, s.cas
FROM Schuze s
WHERE NOT EXISTS (
    SELECT 1
    FROM Ucastni_Se_Alkoholik u
    WHERE u.Schuze = s.ID_Schuze
);

-- Zobrazeni vsech alkoholiku, kteri se zucastnili schuze, kde vedoucim byl odbornik John Doe (pouziti IN)
SELECT DISTINCT a.Jmeno, a.Prijmeni
FROM Alkoholik a
WHERE a.ID_Osoby IN (
    SELECT u.Alkoholik
    FROM Ucastni_Se_Alkoholik u
    WHERE u.Schuze IN (
        SELECT s.ID_Schuze
        FROM Schuze s
        WHERE s.Vedouci_Odbornik IN (SELECT o.ID_Osoby FROM Odbornik o WHERE o.Jmeno = 'Jane' AND o.Prijmeni = 'Smith')
    )
);

-- Zobrazeni poctu schuzi, kterych se zucastnil kazdy alkoholik a zoradeni podle poctu schuzi
-- Muzeme zjistit, kdo je nejaktivnejsi alkoholik (pouziti GROUP BY)
SELECT a.Jmeno, a.Prijmeni, COUNT(u.Schuze) AS Total_Meetings_Attended
FROM Alkoholik a
JOIN Ucastni_Se_Alkoholik u ON a.ID_Osoby = u.Alkoholik
GROUP BY a.Jmeno, a.Prijmeni
ORDER BY Total_Meetings_Attended DESC;


-- Zobrazeni poctu schuzi, ktere se konaly na jednotlivych mistech a zoradeni podle poctu schuzi
-- Muze byt v systemu pouzito na zjisteni popularity mist schuzi
SELECT m.Nazev, COUNT(s.ID_Schuze) AS Total_Meetings
FROM Misto m
JOIN Schuze s ON m.ID_mista = s.Kona_se
GROUP BY m.Nazev
ORDER BY Total_Meetings DESC;

-- NETRIVIALNI DOTAZ S POUZITIM WITH A CASE --

-- Zobrazeni poctu zaznamu v jednotlivych kategoriich podle miry vypiteho alkoholu
-- spolecne s poctem alkoholiku zahrnutych v dane kategorii
WITH Kategorie_vypiteho_alkoholu AS (
    SELECT
        CASE
            WHEN Mira_Alkoholu <= 1 THEN 'Mala'
            WHEN Mira_Alkoholu > 1 AND Mira_Alkoholu <= 3 THEN 'Stredni'
            WHEN Mira_Alkoholu > 3 AND Mira_Alkoholu <= 6 THEN 'Vysoka'
            ELSE 'Extremni'
        END AS Skupina_miry_alkoholu,
        Poradove_Cislo -- Use any column from Kontrola to ensure all rows are counted
    FROM
        Kontrola
)

SELECT
    kva.Skupina_miry_alkoholu,
    COUNT(kva.Poradove_Cislo) AS Pocet_zaznamu,
    COUNT(DISTINCT ka.Testovany_Alkoholik) AS Pocet_alkoholiku
FROM
    Kategorie_vypiteho_alkoholu kva
JOIN
    Kontrola ka ON kva.Poradove_Cislo = ka.Poradove_Cislo
GROUP BY
    kva.Skupina_miry_alkoholu
ORDER BY
    kva.Skupina_miry_alkoholu;

-- INDEX A EXPLAIN PLAN --

CREATE INDEX idx_Mira_Alkoholu ON Kontrola(Mira_Alkoholu);

-- EXPLAIN PLAN FOR s pouzitim indexu idx_Mira_Alkoholu
EXPLAIN PLAN FOR
SELECT a.Jmeno, a.Prijmeni, Mira_Alkoholu
FROM Alkoholik a JOIN Kontrola k ON a.ID_Osoby = k.Testovany_Alkoholik
WHERE Mira_Alkoholu > 1
ORDER BY Mira_Alkoholu DESC;

-- EXPLAIN PLAN FOR bez pouzi indexu
EXPLAIN PLAN FOR
SELECT Typ_Alkoholu, AVG(Mira_Alkoholu) AS Average_Level
FROM Kontrola
GROUP BY Typ_Alkoholu;

-- Zobrazeni planu pro dotaz (vypise posledni plan ulozeny v pameti)
SELECT * FROM TABLE(DBMS_XPLAN.DISPLAY);

-- MATERIALIZED VIEW --

-- Vytvoreni materializovaneho pohledu pro prumerne hodnoty miry alkoholu podle typu alkoholu
CREATE MATERIALIZED VIEW XKOZAN01.Average_Alcohol_Level AS
SELECT Typ_Alkoholu, AVG(Mira_Alkoholu) AS Average_Level
FROM Kontrola
GROUP BY Typ_Alkoholu;


-- Udeleni prav pro pristup k materializovanemu pohledu druhemu clenovi tymu
GRANT SELECT ON XKOZAN01.Average_Alcohol_Level TO XJOUKL00;

-- Ukazka pouziti materializovaneho pohledu
SELECT * FROM XKOZAN01.Average_Alcohol_Level;

-- NETRIVIALNI ULOZNE PROCEDURY --

---------------------------------PROCEDURA 1---------------------------------
-- Procedura pro ziskani informaci o alkoholikovi podle ID
CREATE OR REPLACE PROCEDURE Get_Alkoholik_Info(
    alkoholik_id IN Alkoholik.ID_Osoby%TYPE,
    alkoholik_info OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN alkoholik_info FOR
    SELECT Jmeno, Prijmeni, Pohlavi, Tel_cislo, Email, Adresa, Datum_Posledniho_Uziti
    FROM Alkoholik
    WHERE ID_Osoby = alkoholik_id;
END;
/

-- Ukazka pouziti procedury pro ziskani informaci o alkoholikovi
DECLARE
    TYPE alkoholik_record_type IS RECORD (
        Jmeno Alkoholik.Jmeno%TYPE,
        Prijmeni Alkoholik.Prijmeni%TYPE,
        Pohlavi Alkoholik.Pohlavi%TYPE,
        Tel_cislo Alkoholik.Tel_cislo%TYPE,
        Email Alkoholik.Email%TYPE,
        Adresa Alkoholik.Adresa%TYPE,
        Datum_Posledniho_Uziti Alkoholik.Datum_Posledniho_Uziti%TYPE
    );

    alkoholik_cursor SYS_REFCURSOR;
    alkoholik_record alkoholik_record_type;
BEGIN
    -- Call the procedure and pass the Alkoholik ID
    Get_Alkoholik_Info(1, alkoholik_cursor);

    -- Ziskani informaci o alkoholikovi z vysledku procedury
    LOOP
        FETCH alkoholik_cursor INTO alkoholik_record;
        EXIT WHEN alkoholik_cursor%NOTFOUND;

        -- Zoobrazeni informaci o alkoholikovi
        DBMS_OUTPUT.PUT_LINE('Name: ' || alkoholik_record.Jmeno);
        DBMS_OUTPUT.PUT_LINE('Surname: ' || alkoholik_record.Prijmeni);
    END LOOP;

    CLOSE alkoholik_cursor;
END;
/

---------------------------------PROCEDURA 2---------------------------------

-- Procedura pro aktualizaci emailu alkoholika podle ID
CREATE OR REPLACE PROCEDURE Update_Alkoholik_Email(
    alkoholik_id IN Alkoholik.ID_Osoby%TYPE,
    new_email IN Alkoholik.Email%TYPE
) AS
BEGIN
    -- Aktualizace emailu
    UPDATE Alkoholik
    SET Email = new_email
    WHERE ID_Osoby = alkoholik_id;

    -- Commit the transaction
    COMMIT;
EXCEPTION
    -- Rollback v pripade chyby
    WHEN OTHERS THEN

        ROLLBACK;

        -- Zobrazit chybovou zpravu
        DBMS_OUTPUT.PUT_LINE('An error occurred while updating the email: ' || SQLERRM);
END;
/


-- Ukazka pouziti procedury pro aktualizaci emailu alkoholika
DECLARE
    alkoholik_id Alkoholik.ID_Osoby%TYPE := 1;
    new_email Alkoholik.Email%TYPE := 'new_email@example.com';
BEGIN
    -- Zavolani procedury pro aktualizaci emailu
    Update_Alkoholik_Email(alkoholik_id, new_email);

    -- Zobrazeni aktualizovaneho emailu
    DECLARE
        updated_email Alkoholik.Email%TYPE;
    BEGIN
        SELECT Email INTO updated_email FROM Alkoholik WHERE ID_Osoby = alkoholik_id;

        DBMS_OUTPUT.PUT_LINE('Updated Email: ' || updated_email);
    END;
END;
/

-- PRAVA --

GRANT ALL ON Kontrola TO XKOZAN01;
GRANT ALL ON Dohlizi_Na TO XKOZAN01;
GRANT ALL ON Ucastni_Se_Alkoholik TO XKOZAN01;
GRANT ALL ON Ucastni_Se_Odbornik TO XKOZAN01;
GRANT ALL ON Ucastni_Se_Patron TO XKOZAN01;
GRANT ALL ON Schuze TO XKOZAN01;
GRANT ALL ON Odbornik TO XKOZAN01;
GRANT ALL ON Patron TO XKOZAN01;
GRANT ALL ON Alkoholik TO XKOZAN01;
