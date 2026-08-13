BEGIN;

-- Datos ficticios para desarrollo. Los valores de Tipo corresponden a:
-- 1 Restaurante, 2 Bar, 3 Cafe, 4 Parrilla,
-- 5 Pizzeria, 6 Cerveceria, 7 Otro.

INSERT INTO lugares (
    "Id",
    "Nombre",
    "Descripcion",
    "Direccion",
    "Barrio",
    "CiudadId",
    "Tipo",
    "Latitud",
    "Longitud",
    "Activo")
VALUES
    -- La Plata
    ('a1000000-0000-4000-8000-000000000001', 'Mesa Platense', 'Restaurante ficticio para pruebas.', 'Calle de prueba 1', 'Centro', '57da0788-e24d-4a38-86ac-d4a84b353d57', 1, -34.921450, -57.954530, TRUE),
    ('a1000000-0000-4000-8000-000000000002', 'Cafe del Encuentro', 'Cafe ficticio para reuniones y meriendas.', 'Avenida de prueba 2', 'Centro', '57da0788-e24d-4a38-86ac-d4a84b353d57', 3, -34.918900, -57.958200, TRUE),

    -- City Bell
    ('a1000000-0000-4000-8000-000000000003', 'Pizzas del Camino', 'Pizzeria ficticia para pruebas.', 'Camino de prueba 3', 'Centro', '21cc2931-f8b2-4314-a9f7-d1685bf6aa81', 5, -34.863400, -58.047800, TRUE),
    ('a1000000-0000-4000-8000-000000000004', 'Patio Cervecero City', 'Cerveceria ficticia con patio.', 'Calle de prueba 4', 'Las Banderitas', '21cc2931-f8b2-4314-a9f7-d1685bf6aa81', 6, -34.859900, -58.052100, TRUE),

    -- Berisso
    ('a1000000-0000-4000-8000-000000000005', 'Parrilla del Puerto', 'Parrilla ficticia para grupos.', 'Avenida de prueba 5', 'Centro', '47695c12-8476-4b58-aa90-5b36d693e179', 4, -34.873500, -57.883500, TRUE),
    ('a1000000-0000-4000-8000-000000000006', 'Bar del Inmigrante', 'Bar ficticio para pruebas.', 'Calle de prueba 6', 'Barrio Banco Provincia', '47695c12-8476-4b58-aa90-5b36d693e179', 2, -34.868900, -57.889200, TRUE),

    -- Villa Elisa
    ('a1000000-0000-4000-8000-000000000007', 'Rincon de Villa Elisa', 'Restaurante ficticio de ambiente familiar.', 'Camino de prueba 7', 'Centro', '90709a3c-c86d-4760-8f53-f763e16f21c3', 1, -34.847100, -58.079300, TRUE),
    ('a1000000-0000-4000-8000-000000000008', 'Cafe de la Estacion', 'Cafe ficticio para desayunos y meriendas.', 'Calle de prueba 8', 'Centro', '90709a3c-c86d-4760-8f53-f763e16f21c3', 3, -34.844200, -58.075500, TRUE),

    -- Los Hornos
    ('a1000000-0000-4000-8000-000000000009', 'La Pizza de Los Hornos', 'Pizzeria ficticia para pruebas.', 'Avenida de prueba 9', 'Centro', '56e291de-c0cd-4abd-877c-4ca4f7aab5ee', 5, -34.950300, -57.985100, TRUE),
    ('a1000000-0000-4000-8000-000000000010', 'Brasas del Sur', 'Parrilla ficticia para encuentros.', 'Calle de prueba 10', 'Los Hornos', '56e291de-c0cd-4abd-877c-4ca4f7aab5ee', 4, -34.955100, -57.980400, TRUE),

    -- Ensenada
    ('a1000000-0000-4000-8000-000000000011', 'Puerto Bar', 'Bar ficticio para pruebas.', 'Calle de prueba 11', 'Centro', 'ef5a3d04-e58d-44f9-9678-eec628961f43', 2, -34.861700, -57.911300, TRUE),
    ('a1000000-0000-4000-8000-000000000012', 'Cerveceria del Canal', 'Cerveceria ficticia para grupos.', 'Avenida de prueba 12', 'El Dique', 'ef5a3d04-e58d-44f9-9678-eec628961f43', 6, -34.865200, -57.916000, TRUE),

    -- Bartolome Bavio
    ('a1000000-0000-4000-8000-000000000013', 'Almacen de Bavio', 'Lugar ficticio de comidas regionales.', 'Ruta de prueba 13', 'Centro', '5f47be23-22bd-4028-a010-b6ca21a6d444', 7, -35.066700, -57.750000, TRUE),
    ('a1000000-0000-4000-8000-000000000014', 'Parrilla La Tranquera', 'Parrilla ficticia de ambiente rural.', 'Camino de prueba 14', 'Zona Rural', '5f47be23-22bd-4028-a010-b6ca21a6d444', 4, -35.071000, -57.744800, TRUE),

    -- Magdalena
    ('a1000000-0000-4000-8000-000000000015', 'Sabores de Magdalena', 'Restaurante ficticio para pruebas.', 'Calle de prueba 15', 'Centro', '0e5e11fd-b744-4d61-8753-8217091023da', 1, -35.080700, -57.517500, TRUE),
    ('a1000000-0000-4000-8000-000000000016', 'Cafe de la Plaza', 'Cafe ficticio frente a una plaza.', 'Calle de prueba 16', 'Centro', '0e5e11fd-b744-4d61-8753-8217091023da', 3, -35.084100, -57.512900, TRUE),

    -- Manuel B. Gonnet
    ('a1000000-0000-4000-8000-000000000017', 'Gonnet Cervecero', 'Cerveceria ficticia para pruebas.', 'Camino de prueba 17', 'Centro', '79be4503-50f7-42c6-b3de-ae94c62ea2cb', 6, -34.889000, -58.018000, TRUE),
    ('a1000000-0000-4000-8000-000000000018', 'Pizza y Amigos', 'Pizzeria ficticia pensada para grupos.', 'Calle de prueba 18', 'Manuel B. Gonnet', '79be4503-50f7-42c6-b3de-ae94c62ea2cb', 5, -34.885600, -58.022400, TRUE),

    -- Tolosa
    ('a1000000-0000-4000-8000-000000000019', 'Bar de Tolosa', 'Bar ficticio para pruebas.', 'Avenida de prueba 19', 'Tolosa', '75d866ec-aa82-4872-9316-9a67c923fa69', 2, -34.900300, -57.995100, TRUE),
    ('a1000000-0000-4000-8000-000000000020', 'El Patio Tolosano', 'Lugar ficticio de comidas variadas.', 'Calle de prueba 20', 'Tolosa', '75d866ec-aa82-4872-9316-9a67c923fa69', 7, -34.896800, -57.990700, TRUE)
ON CONFLICT ("Id") DO UPDATE SET
    "Nombre" = EXCLUDED."Nombre",
    "Descripcion" = EXCLUDED."Descripcion",
    "Direccion" = EXCLUDED."Direccion",
    "Barrio" = EXCLUDED."Barrio",
    "CiudadId" = EXCLUDED."CiudadId",
    "Tipo" = EXCLUDED."Tipo",
    "Latitud" = EXCLUDED."Latitud",
    "Longitud" = EXCLUDED."Longitud",
    "Activo" = EXCLUDED."Activo";

COMMIT;
