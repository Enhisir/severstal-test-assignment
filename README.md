### Тестовое задание №2
Склад рулонов металла

**Cборка**

```shell
docker compose build
```

**Запуск**

```shell
docker compose up # запуск сервисов
dotnet ef database update --project TestAssignment # применение миграций
```

**Scalar API доступен по адресу `/scalar`**