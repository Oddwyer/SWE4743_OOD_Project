## Agreed upon API endpoints:

| Purpose | Color                                         | Usage                   |
| ------- | --------------------------------------------- | ----------------------- |
| GET     | /api/devices                                  | list devices            |
| GET     | /api/devices/{id}                             | get device              |
| POST    | /api/devices                                  | register device         |
| DELETE  | /api/devices/{id}                             | remove device           |
| POST    | /api/devices/{id}/commands                    | control device          |
| GET     | /api/devices/{id}/history                     | see command history     |
| PUT     | /api/locations/{location}/ambient-temperature | set ambient temperature |
| PUT     | /api/simulation/speed                         | set simulation speed    |
| POST    | /api/simulation/reset                         | simulation reset        |
