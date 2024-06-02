
# Телеграм-бот для створення демотиваторів

Цей телеграм-бот дозволяє користувачам створювати демотиваційні постери, комбінуючи зображення та текст.

## Особливості

- **Створення демотиваторів:** Користувачі можуть надсилати зображення разом із текстом для створення демотиваційних постерів.
- **Інтерактивний інтерфейс:** Використовує клавіатуру Telegram для зручної навігації.
- **Налаштування:** Дозволяє користувачам додавати свій текст до зображень.

## Початок роботи

Для використання цього бота, слід дотримуватися таких кроків:

1. **Склонуйте репозиторій:**
   ```
   git clone https://github.com/donutkrll/Lab2TelegramBot.git
   ```

2. **Встановіть залежності:**
   ```
   cd Lab2Telegrambot
   dotnet restore
   ```

3. **Налаштуйте токен:**
   Замініть плейсхолдер токену у файлі `Program.cs` на ваш токен API телеграм-бота.

4. **Запустіть бота:**
   ```
   dotnet run
   ```

## Використання

1. Розпочніть роботу з ботом, відправивши `/start`.
2. Використайте команду `/memes`, щоб розпочати процес створення демотиватора.
3. Надішліть зображення разом з бажаним текстом.
4. Отримайте демотиваційний постер у відповідь.
