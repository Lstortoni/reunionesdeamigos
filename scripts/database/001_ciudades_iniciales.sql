BEGIN;

INSERT INTO ciudades ("Id", "Nombre", "Provincia", "Pais", "Activa")
VALUES
    ('57da0788-e24d-4a38-86ac-d4a84b353d57', 'La Plata', 'Buenos Aires', 'Argentina', TRUE),
    ('21cc2931-f8b2-4314-a9f7-d1685bf6aa81', 'City Bell', 'Buenos Aires', 'Argentina', TRUE),
    ('47695c12-8476-4b58-aa90-5b36d693e179', 'Berisso', 'Buenos Aires', 'Argentina', TRUE),
    ('90709a3c-c86d-4760-8f53-f763e16f21c3', 'Villa Elisa', 'Buenos Aires', 'Argentina', TRUE),
    ('56e291de-c0cd-4abd-877c-4ca4f7aab5ee', 'Los Hornos', 'Buenos Aires', 'Argentina', TRUE),
    ('ef5a3d04-e58d-44f9-9678-eec628961f43', 'Ensenada', 'Buenos Aires', 'Argentina', TRUE),
    ('5f47be23-22bd-4028-a010-b6ca21a6d444', 'Bartolomé Bavio', 'Buenos Aires', 'Argentina', TRUE),
    ('0e5e11fd-b744-4d61-8753-8217091023da', 'Magdalena', 'Buenos Aires', 'Argentina', TRUE),
    ('79be4503-50f7-42c6-b3de-ae94c62ea2cb', 'Manuel B. Gonnet', 'Buenos Aires', 'Argentina', TRUE),
    ('75d866ec-aa82-4872-9316-9a67c923fa69', 'Tolosa', 'Buenos Aires', 'Argentina', TRUE)
ON CONFLICT ("Pais", "Provincia", "Nombre")
DO UPDATE SET "Activa" = TRUE;

COMMIT;
