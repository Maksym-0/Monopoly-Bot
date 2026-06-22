# 🎲 Monopoly Telegram Bot Client

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Telegram API](https://img.shields.io/badge/Telegram_Bot_API-2CA5E0?style=for-the-badge&logo=telegram&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)

Інтерактивний Telegram-бот, який виступає в ролі повноцінного клієнта для гри в Монополію. Бот розроблено з дотриманням **Clean Architecture** та паттернів проєктування.

👉 **Бекенд API:** [Monopoly REST API](https://github.com/Maksym-0/Monopoly-API)

## 🛠 Стек технологій
- **Мова та фреймворк:** C#, .NET 8, .NET 8 Console Application
- **База даних:** PostgreSQL, Entity Framework Core (Code-First)
- **Автентифікація:** JWT (JSON Web Token) Bearer

---

## 📌 Головний функціонал
- **Інтерактивний UI:** Використання `ReplyKeyboardMarkup` та `InlineKeyboardMarkup` для керування ігровим процесом (кидок кубиків, торгівля, купівля нерухомості).
- **Система розсилок повідомлень (Broadcast):** Сервіс персоналізованих сповіщень для всіх учасників гри та глядачів (наприклад, "Гравець X сплатив ренту гравцю Y").
- **Створення акаунту, кімнати та гри:** Покрокова система створення акаунту, кімнати, гри та торгових пропозицій між гравцями.
- **Режим глядача:** Можливість приєднатися до активної кімнати виключно для спостереження за ходом гри.

---

## 🏗 Чиста Архітектура (Clean Architecture)

Проєкт розділено на шари для відділення логіки Telegram від бізнес-правил клієнта:

- **`.Telegram` (Presentation Layer):** Відповідає за прийом Update-ів від Telegram, керування станами чату та розсилку повідомлень. Логіка маршрутизується між спеціалізованими обробниками (`CommandHandlers`, `StatusHandlers`, `CallbackHandlers`).
- **`.Application` (Service Layer):** Оркеструє виклики до зовнішнього REST API та керує локальним станом користувача (авторизація, перевірка актуальності JWT).
- **`.Core` (Domain Layer):** Містить моделі стану (`ChatStatus`, `User`), DTO для спілкування з API, перелічення (enums `BotState` та `ErrorType`) та контракти інтерфейсів.
- **`.DataAccess` (Infrastructure Layer):** 
  - **`Postgres`**: Інкапсулює локальну БД бота для збереження сесій та статусів чату через патерни Repository та Unit of Work.
  - **`ApiClients`**: Clients для HTTP-взаємодії з бекендом.

---

## 💎 Архітектурні рішення та Патерни

<details>
<summary><b>Керування станом (State Management) через Базу Даних</b></summary>
Замість збереження станів користувачів (наприклад очікування вводу пароля) у статичних словниках в оперативній пам'яті, бот використовує таблицю <code>ChatStatuses</code>. Це гарантує, що при перезапуску бота сесії користувачів не втратяться. Сама модель <code>ChatStatus</code> інкапсулює логіку перевірок (наприклад, метод <code>IsTradeInProgress()</code>), що розвантажує обробники від зайвих перевірок.
</details>

<details>
<summary><b>Single Responsibility (SRP)</b></summary>
Вхідне повідомлення аналізується і передається у відповідний сервіс: <code>CommandHandlers</code> (для звичайних кнопок), <code>StatusHandlers</code> (якщо юзер знаходиться у процесі вводу даних, наприклад, пароля), або <code>CallbackHandlers</code> (для Inline-кнопок). Кожен клас має єдину відповідальність.
</details>

<details>
<summary><b>Асинхронний Broadcast (Task.WhenAll)</b></summary>
Завдяки потокобезпечності ITelegramBotClient для розсилки сповіщень усім гравцям у кімнаті використовується метод <code>await Task.WhenAll(tasks)</code>, де <code>tasks</code> є колекцією асинхронних запитів на відправку повідомлень. Мережеві запити до Telegram API виконуються паралельно, що гарантує миттєву доставку повідомлень.
</details>

<details>
<summary><b>Result Pattern</b></summary>
Бот використовує власну обгортку <code>ServiceResponse&lt;T&gt;</code>. Це дозволяє безпечно обробляти помилки бізнес-логіки (наприклад, "Недостатньо коштів") чи помилки бізнес-логіки бота (наприклад, "Час авторизації вичерпано") на рівні <code>.Application</code> і повертати користувачу зрозумілі повідомлення в чат, уникаючи Exceptions.
</details>

---

## 🚀 Як запустити локально

1. Знайдіть [@BotFather](https://t.me/botfather) у Telegram та створіть токен для свого бота.

2. Клонуйте цей репозиторій:
   ```bash
   git clone https://github.com/Maksym-0/Monopoly-Bot.git
   ```
3. Вкажіть токен Telegram, рядок підключення до локальної PostgreSQL та адреси хостів вашого Monopoly API у файлі appsettings.json (або secrets.json). 

4. Застосуйте міграції для створення локальної БД бота:
    ```bash
    Update-Database
    ```

5. Переконайтеся, що Monopoly REST API запущено, та стартуйте проєкт бота:
    ```bash
    dotnet run
    ```
