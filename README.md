### Подготовка к запуску
```bash
git clone https://github.com/Snark1976/stats_hub_test.git
cd .\stats_hub_test
```
### Прогон тестов
```bash
dotnet test
```
### Запуск
```bash
docker compose up --build
```
База создается и миграции применяются автоматически при запуске


Сваггер: http://localhost:8080/swagger/index.html

Страница с графиком выручки: http://localhost:8081/DailyStats


