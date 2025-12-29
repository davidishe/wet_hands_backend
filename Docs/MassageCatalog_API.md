# API каталог массажных салонов

Ниже — короткий гайд по запросам, которые сейчас доступны мобильному фронтенду.

## Публичные endpoints каталога
- `GET /api/massageplaces` — постраничный список карточек.
  - query: `q`, `categories` (через запятую), `country`, `city`, `minRating`, `maxRating`, `offset`, `limit` (по умолчанию 200, максимум 500), `includeMainImage`, `includeGallery`.
- `GET /api/massageplaces/details?name=<string>` — получение карточки по имени (регистронезависимо).
  - query: `includeMainImage`, `includeGallery` — можно выключить тяжёлые поля.
- `GET /api/massageplaces/categories` — сгруппированные типы услуг, доступные в каталоге (только то, что реально встречается в базе).
- `GET /api/massageplaces/filterOptions` — выпадающие списки стран/городов/категорий и диапазон рейтинга.

## Управление каталогом
(требует авторизации — используется глобальная политика `RequireAuthenticatedUser`)

- `POST /api/manager/catalog`
  - Body (JSON): `{ "name": "…", "description": "…", "rating": 0-100, "country": "…", "city": "…", "mainImage": "base64…", "gallery": ["base64…"], "attributes": ["тайский массаж", …] }`
  - Повтор имени вернёт `409 Conflict`.
- `PUT /api/manager/catalog/{id}` — обновление существующей записи по `id` с тем же телом, что и на create.
  - Также валидирует уникальность имени.

## Замечания для фронта
- Поле `rating` всегда в диапазоне 0–100 (сервер зажимает значения).
- Если в БД не указана страна, сервер вернёт `Russia` (совместимость со старой выборкой).
- `gallery` и `attributes` всегда массивы (если нет данных — пустые массивы).
