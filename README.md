### System Rezerwacji Lotów - Magic Lines

Witaj w naszym programie do rezerwacji lotów! Ten program umożliwia użytkownikom wybór lotu i zarządzanie rezerwacjami biletów za pomocą prostego interfejsu tekstowego. Poniżej znajdziesz szczegółowy opis funkcji oraz instrukcję obsługi.

#### Diagram UML

![UML](https://github.com/sikl0/magiclines/assets/45985086/4785e64b-81ae-4b33-afc3-e52e97f4650a)

#### Wykorzystane technologie
![csharp](https://github.com/sikl0/magiclines/assets/45985086/64251bfe-714b-4019-885e-33c502ee0499)



#### Funkcje

1. **System rejestracji**
   - Użytkownicy mogą się zarejestrować
   - Po rejestracji użytkownicy mogą się zalogować
   - Możliwa jest również zmiana danych i hasła oraz sprawdzenie swoich rezerwacji
     
1. **Rezerwacja Lotu**
   - Użytkownicy mogą wybrać dostępne trasy lotów.
   - Wybierają klasę biletu (Economy, Economy (Okno), Economy Plus, Business).
   - Podają swoje dane osobowe (imię, nazwisko, email) w celu dokonania rezerwacji.

2. **Zarządzanie Biletami**
   - Użytkownicy mogą sprawdzić szczegóły zarezerwowanego biletu za pomocą jego unikalnego numeru.
   - Mogą również anulować rezerwację, co wiąże się z opłatą za anulację.

3. **Zachowanie Danych**
   - Lista dostępnych miejsc (`flight_data.txt`) są ładowane przy starcie programu.
   - Dane o biletach (zarezerwowane bilety) są zapisywane w formacie JSON i przechowywane w pliku (`tickets_data.json`).
   - Dane o użytkownikach są szyfrowane i przechowywane w pliku JSON (`users.json`). 

#### Opcje Menu

Po uruchomieniu programu oraz zalogowaniu się, bądź rejestracji użytkownicy mają do dyspozycji następujące opcje:

- **1. Zarezerwuj lot**
  - Pozwala na dokonanie rezerwacji biletu na wybrany lot.

- **2. Pokaż moje rezerwacje**
  - Umożliwia sprawdzenie szczegółów zarezerwowanego biletu oraz zarządzanie rezerwacjami.
 
- **3. Zmień hasło**
  - Umożliwia zmianę hasła
 
- **4. Zmień email**
  - Umożliwia zmianę adresu email
 
- **5. Wyloguj się**
  - Umożliwia wylogowanie się

- **0. Wyjdź**
  - Kończy działanie programu i zapisuje wszystkie dane.

#### Jak korzystać

1. **Rezerwacja Lotu**
   - Wybierz opcję `1`.
   - Wybierz interesującą Cię trasę lotu z listy.
   - Wybierz klasę biletu (Economy, Economy (Okno), Economy Plus, Business).

2. **Sprawdzanie/Anulowanie Rezerwacji**
   - Wybierz opcję `2`.
   - Podaj numer biletu, aby sprawdzić jego szczegóły.
   - Opcja `1` pozwala na anulowanie rezerwacji, z wyświetloną opłatą za anulację.

3. **Zakończenie Programu**
   - Wybierz opcję `0`, aby wyjść z programu. Wszystkie dane zostaną automatycznie zapisane.

#### Struktura Plików

- **`flight_data.txt`**
  - Przechowuje szczegóły lotów, w tym trasę, cenę podstawową.
  - Format danych: `nazwaTrasy|cenaPodstawowa|ilość,Miejsc,W,Poszczególnych,Klasach`

- **`tickets_data.json`**
  - Przechowuje zarezerwowane bilety w formacie JSON, zawierające numer biletu, trasę, klasę miejsc, cenę oraz dane pasażera.

- **`users.json`**
  - Przechowuje zaszyfrowane dane użytkowników w formacie JSON, zawierające login, hasło, email i ilość punktów lojalnościowych

#### Uwagi

- Program obsługuje walidację danych wprowadzanych przez użytkownika.
- Ceny biletów są obliczane dynamicznie na podstawie ceny podstawowej i mnożnika.
- Anulowanie rezerwacji wiąże się z opłatą, którą stanowi 10% ceny biletu.

### Zależności

Ten program korzysta z następujących bibliotek:
- `System.IO` do operacji na plikach.
- `System.Text.Json` do serializacji danych w formacie JSON.
- `BCrypt.Net-Next` do hashowania haseł.
- `Newtonsoft.Json` do obsługi plików JSON.

### Konfiguracja

Upewnij się, że w folderze z programem znajdują się następujące pliki:
- `tickets_data.json` (początkowo pusty lub utworzony przez program)
- `flight_data.txt`(Liczba aktulanie dostępnych miejsc, jest tworzony przez program gdy go nie ma i na bieżąco aktualizowany przez program)

### Autorzy

Program został napisany przez grupę 3 studentów I roku kierunku Informatyka z Uniwersytetu WSB Merito w Chorzowie:
- Mateusz Nocoń
- Patryk Otrębski
- Kacper Olkis
  
