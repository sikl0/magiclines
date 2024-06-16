### System Rezerwacji Lotów - Magic Lines

Witaj w naszym programie do rezerwacji lotów! Ten program umożliwia użytkownikom wybór lotu i zarządzanie rezerwacjami biletów za pomocą prostego interfejsu tekstowego. Poniżej znajdziesz szczegółowy opis funkcji oraz instrukcję obsługi.

#### Funkcje

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
   - Dane o lotach (trasy, ceny podstawowe) są przechowywane w pliku tekstowym (`flights.txt`). 

#### Opcje Menu

Po uruchomieniu programu użytkownicy mają do dyspozycji następujące opcje:

- **1. Zarezerwuj lot**
  - Pozwala na dokonanie rezerwacji biletu na wybrany lot.

- **2. Sprawdź lot**
  - Umożliwia sprawdzenie szczegółów zarezerwowanego biletu oraz zarządzanie rezerwacjami.

- **0. Wyjdź**
  - Kończy działanie programu i zapisuje wszystkie dane.

#### Jak korzystać

1. **Rezerwacja Lotu**
   - Wybierz opcję `1`.
   - Wybierz interesującą Cię trasę lotu z listy.
   - Wybierz klasę biletu (Economy, Economy (Okno), Economy Plus, Business).
   - Podaj swoje dane osobowe, gdy zostaniesz o to poproszony.

2. **Sprawdzanie/Anulowanie Rezerwacji**
   - Wybierz opcję `2`.
   - Podaj numer biletu, aby sprawdzić jego szczegóły.
   - Opcja `1` pozwala na anulowanie rezerwacji, z wyświetloną opłatą za anulację.

3. **Zakończenie Programu**
   - Wybierz opcję `0`, aby wyjść z programu. Wszystkie dane zostaną automatycznie zapisane.

#### Struktura Plików

- **`flights.txt`**
  - Przechowuje szczegóły lotów, w tym trasę, cenę podstawową.
  - Format danych: `numerTrasy,nazwaTrasy,cenaPodstawowa`

- **`tickets_data.json`**
  - Przechowuje zarezerwowane bilety w formacie JSON, zawierające numer biletu, trasę, klasę miejsc, cenę oraz dane pasażera.

- **`flight_data.txt`**
  - Przechowuje lisczbę dostępnych miejsc.
  - Format danych:`miejscaBiznes,miejscaEconomyPlus,miejscaEconomy,miejscaEconomyOkno`

#### Uwagi

- Program obsługuje walidację danych wprowadzanych przez użytkownika.
- Ceny biletów są obliczane dynamicznie na podstawie ceny podstawowej i mnożnika.
- Anulowanie rezerwacji wiąże się z opłatą, którą stanowi 10% ceny biletu.

### Zależności

Ten program korzysta z następujących bibliotek:
- `System.IO` do operacji na plikach.
- `System.Text.Json` do serializacji danych w formacie JSON.

### Konfiguracja

Upewnij się, że w folderze z programem znajdują się następujące pliki:
- `flights.txt` (początkowo pusty lub utworzony przez program, format danych jak powyżej)
- `tickets_data.json` (początkowo pusty lub utworzony przez program)
- `flight_data.txt`(Liczba aktulanie dostpnych miejsc, jest tworzony przez program gdy go nie ma i na bieżąco aktualizowany przez program)
  
