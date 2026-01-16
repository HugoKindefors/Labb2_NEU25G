# Music Library Manager – Labb 2 Databaser

Detta är en WPF-applikation byggd i **C#** med **Entity Framework Core** som en del av **Labb 2 – Databaser**.

Applikationen är kopplad mot den tillhandahållna **everyloop**-databasen och låter användaren hantera artister, album, låtar och spellistor.

---

## 📌 Funktionalitet

### Artister & Album
- Visa artister och deras album i en TreeView
- Lägg till, redigera och ta bort artister
- Lägg till, redigera och ta bort album

### Låtar
- Visa låtar för ett valt album
- Lägg till nya låtar till ett album
- Redigera låtinformation (titel, kompositör, längd)
- Ta bort låtar  
  - Låten tas automatiskt bort från alla spellistor

### Spellistor
- Skapa, byta namn på och ta bort spellistor
- Lägg till låtar i spellistor
- Ta bort låtar från spellistor
- Visa låtar i vald spellista

---

## ▶️ Starta applikationen

1. Klona repot
2. Öppna lösningen i **Visual Studio**
3. Kontrollera att du har:
   - SQL Server installerat
   - Tillgång till `everyloop`-databasen
4. Kontrollera connection string i `MusicContext.cs`
5. Kör applikationen (`F5`)

---

## 🧪 Testa applikationen

Förslag på testflöde:
1. Lägg till en ny artist
2. Lägg till ett album till artisten
3. Lägg till låtar till albumet
4. Redigera en låt
5. Skapa en spellista och lägg till låtar
6. Ta bort en låt från en spellista
7. Ta bort en låt helt (kontrollera att den även försvinner ur spellistor)
8. Ta bort album och artist efter att låtarna är borttagna


