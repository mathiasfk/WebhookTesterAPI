CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;
CREATE TABLE "Tokens" (
    "Id" uuid NOT NULL,
    "Created" timestamp with time zone NOT NULL,
    "LastUsed" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Tokens" PRIMARY KEY ("Id")
);

CREATE TABLE "Webhooks" (
    "Id" uuid NOT NULL,
    "OwnerToken" uuid NOT NULL,
    "Created" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Webhooks" PRIMARY KEY ("Id")
);

CREATE TABLE "WebhookRequests" (
    "Id" uuid NOT NULL,
    "WebhookId" uuid NOT NULL,
    "HttpMethod" text NOT NULL,
    "Body" text NOT NULL,
    "ReceivedAt" timestamp with time zone NOT NULL,
    "Headers" text NOT NULL,
    CONSTRAINT "PK_WebhookRequests" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_WebhookRequests_Webhooks_WebhookId" FOREIGN KEY ("WebhookId") REFERENCES "Webhooks" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_WebhookRequests_WebhookId" ON "WebhookRequests" ("WebhookId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250324014756_InitialCreate', '9.0.3');

COMMIT;

