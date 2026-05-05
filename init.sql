/*==============================================================*/
/* Table: Approach                                              */
/*==============================================================*/
create table Approach (
   Technic_ID           INT4                 not null,
   RepairParts_ID       INT4                 not null,
   constraint PK_APPROACH primary key (Technic_ID, RepairParts_ID)
);

/*==============================================================*/
/* Index: Approach_PK                                           */
/*==============================================================*/
create unique index Approach_PK on Approach (
Technic_ID,
RepairParts_ID
);

/*==============================================================*/
/* Index: Approach2_FK                                          */
/*==============================================================*/
create  index Approach2_FK on Approach (
RepairParts_ID
);

/*==============================================================*/
/* Index: Approach_FK                                           */
/*==============================================================*/
create  index Approach_FK on Approach (
Technic_ID
);

/*==============================================================*/
/* Table: Client                                                */
/*==============================================================*/
create table Client (
   Client_ID            SERIAL               not null,
   Client_Name          VARCHAR(64)          not null,
   Client_Surname       VARCHAR(64)          not null,
   Client_Patronymic    VARCHAR(64)          null,
   Client_PhoneNumber   CHAR(11)             not null,
   Client_Adress        VARCHAR(128)         null,
   constraint PK_CLIENT primary key (Client_ID)
);

/*==============================================================*/
/* Index: Client_PK                                             */
/*==============================================================*/
create unique index Client_PK on Client (
Client_ID
);

/*==============================================================*/
/* Table: Completed_works                                       */
/*==============================================================*/
create table Completed_works (
   Order_ID             INT4                 not null,
   Staff_ID             INT4                 not null,
   ComplWorks_Datetime  DATE                 not null,
   ComplWorks_JobContent VARCHAR(256)         null,
   ComplWorks_Duration  DATE                 null,
   constraint PK_COMPLETED_WORKS primary key (Order_ID, Staff_ID, ComplWorks_Datetime)
);

/*==============================================================*/
/* Index: Completed_works_PK                                    */
/*==============================================================*/
create unique index Completed_works_PK on Completed_works (
Order_ID,
Staff_ID,
ComplWorks_Datetime
);

/*==============================================================*/
/* Index: Perform_FK                                            */
/*==============================================================*/
create  index Perform_FK on Completed_works (
Staff_ID
);

/*==============================================================*/
/* Index: Contains_FK                                           */
/*==============================================================*/
create  index Contains_FK on Completed_works (
Order_ID
);

/*==============================================================*/
/* Table: Delivers                                              */
/*==============================================================*/
create table Delivers (
   RepairParts_ID       INT4                 not null,
   Supplier_Company     VARCHAR(256)         not null,
   constraint PK_DELIVERS primary key (RepairParts_ID, Supplier_Company)
);

/*==============================================================*/
/* Index: Delivers_PK                                           */
/*==============================================================*/
create unique index Delivers_PK on Delivers (
RepairParts_ID,
Supplier_Company
);

/*==============================================================*/
/* Index: Delivers2_FK                                          */
/*==============================================================*/
create  index Delivers2_FK on Delivers (
Supplier_Company
);

/*==============================================================*/
/* Index: Delivers_FK                                           */
/*==============================================================*/
create  index Delivers_FK on Delivers (
RepairParts_ID
);

/*==============================================================*/
/* Table: HistotySupply                                         */
/*==============================================================*/
create table HistotySupply (
   Supply_ID            SERIAL               not null,
   RepairParts_ID       INT4                 not null,
   Supplier_Company     VARCHAR(256)         not null,
   Supply_Date          DATE                 not null,
   Supply_Quantity      INT2                 not null,
   Supply_Cost          MONEY                not null,
   constraint PK_HISTOTYSUPPLY primary key (Supply_ID)
);

/*==============================================================*/
/* Index: HistotySupply_PK                                      */
/*==============================================================*/
create unique index HistotySupply_PK on HistotySupply (
Supply_ID
);

/*==============================================================*/
/* Index: Have3_FK                                              */
/*==============================================================*/
create  index Have3_FK on HistotySupply (
RepairParts_ID
);

/*==============================================================*/
/* Index: Have2_FK                                              */
/*==============================================================*/
create  index Have2_FK on HistotySupply (
Supplier_Company
);

/*==============================================================*/
/* Table: "Order"                                               */
/*==============================================================*/
create table "Order" (
   Order_ID             SERIAL               not null,
   Staff_ID             INT4                 not null,
   Client_ID            INT4                 not null,
   Technic_ID           INT4                 not null,
   Order_DateCreation   DATE                 not null,
   Order_status         INT4                 not null default 0
      constraint CKC_ORDER_STATUS_ORDER check (Order_status between 0 and 1),
   Order_DateCompletion DATE                 null,
   Order_Description    VARCHAR(256)         null,
   Order_Cost           MONEY                null,
   Order_Mileage        INT4                 not null,
   constraint PK_ORDER primary key (Order_ID)
);

comment on column "Order".Order_status is
'0 - Заказ невыполнен
1 - Заказ выполнен';

/*==============================================================*/
/* Index: Order_PK                                              */
/*==============================================================*/
create unique index Order_PK on "Order" (
Order_ID
);

/*==============================================================*/
/* Index: Register_FK                                           */
/*==============================================================*/
create  index Register_FK on "Order" (
Staff_ID
);

/*==============================================================*/
/* Index: Create_FK                                             */
/*==============================================================*/
create  index Create_FK on "Order" (
Client_ID
);

/*==============================================================*/
/* Index: Includes_FK                                           */
/*==============================================================*/
create  index Includes_FK on "Order" (
Technic_ID
);

/*==============================================================*/
/* Table: RepairParts                                           */
/*==============================================================*/
create table RepairParts (
   RepairParts_ID       SERIAL               not null,
   RepairParts_Title    VARCHAR(64)          not null,
   RepairParts_Marks    VARCHAR(64)          not null,
   RepairParts_Model    VARCHAR(128)         not null,
   RepairParts_Quantity INT2                 not null,
   constraint PK_REPAIRPARTS primary key (RepairParts_ID)
);

/*==============================================================*/
/* Index: RepairParts_PK                                        */
/*==============================================================*/
create unique index RepairParts_PK on RepairParts (
RepairParts_ID
);

/*==============================================================*/
/* Table: Staff                                                 */
/*==============================================================*/
create table Staff (
   Staff_ID             SERIAL               not null,
   Staff_Name           VARCHAR(64)          not null,
   Staff_Surname        VARCHAR(64)          not null,
   Staff_Patronymic     VARCHAR(64)          null,
   Staff_Post           VARCHAR(64)          not null,
   Staff_PhoneNumber    CHAR(11)             not null,
   Staff_Adress         VARCHAR(128)         not null,
   constraint PK_STAFF primary key (Staff_ID)
);

/*==============================================================*/
/* Index: Staff_PK                                              */
/*==============================================================*/
create unique index Staff_PK on Staff (
Staff_ID
);

/*==============================================================*/
/* Table: Supplier                                              */
/*==============================================================*/
create table Supplier (
   Supplier_Company     VARCHAR(256)         not null,
   Supplier_Adress      VARCHAR(128)         not null,
   Supplier_PhoneNumber CHAR(11)             not null,
   constraint PK_SUPPLIER primary key (Supplier_Company)
);

/*==============================================================*/
/* Index: Supplier_PK                                           */
/*==============================================================*/
create unique index Supplier_PK on Supplier (
Supplier_Company
);

/*==============================================================*/
/* Table: Technic                                               */
/*==============================================================*/
create table Technic (
   Technic_ID           SERIAL               not null,
   Technic_Mark         VARCHAR(64)          not null,
   Technic_Model        VARCHAR(128)         not null,
   Technic_SerialNumber CHAR(17)             not null,
   Technic_DateProduction INT2                 not null,
   Technic_Condition    VARCHAR(128)         null,
   Technic_Self         INT4                 not null default 1
      constraint CKC_TECHNIC_SELF_TECHNIC check (Technic_Self between 0 and 1),
   Technic_Mileage      INT4                 not null,
   constraint PK_TECHNIC primary key (Technic_ID)
);

comment on column Technic.Technic_Self is
'0 - Не собственная
1 - Собственная';

/*==============================================================*/
/* Index: Technic_PK                                            */
/*==============================================================*/
create unique index Technic_PK on Technic (
Technic_ID
);

/*==============================================================*/
/* Table: Uses                                                  */
/*==============================================================*/
create table Uses (
   Order_ID             INT4                 not null,
   RepairParts_ID       INT4                 not null,
   constraint PK_USES primary key (Order_ID, RepairParts_ID)
);

/*==============================================================*/
/* Index: Uses_PK                                               */
/*==============================================================*/
create unique index Uses_PK on Uses (
Order_ID,
RepairParts_ID
);

/*==============================================================*/
/* Index: Uses2_FK                                              */
/*==============================================================*/
create  index Uses2_FK on Uses (
RepairParts_ID
);

/*==============================================================*/
/* Index: Uses_FK                                               */
/*==============================================================*/
create  index Uses_FK on Uses (
Order_ID
);

alter table Approach
   add constraint FK_APPROACH_APPROACH_TECHNIC foreign key (Technic_ID)
      references Technic (Technic_ID)
      on delete restrict on update restrict;

alter table Approach
   add constraint FK_APPROACH_APPROACH2_REPAIRPA foreign key (RepairParts_ID)
      references RepairParts (RepairParts_ID)
      on delete restrict on update restrict;

alter table Completed_works
   add constraint FK_COMPLETE_CONTAINS_ORDER foreign key (Order_ID)
      references "Order" (Order_ID)
      on delete restrict on update restrict;

alter table Completed_works
   add constraint FK_COMPLETE_PERFORM_STAFF foreign key (Staff_ID)
      references Staff (Staff_ID)
      on delete restrict on update restrict;

alter table Delivers
   add constraint FK_DELIVERS_DELIVERS_REPAIRPA foreign key (RepairParts_ID)
      references RepairParts (RepairParts_ID)
      on delete restrict on update restrict;

alter table Delivers
   add constraint FK_DELIVERS_DELIVERS2_SUPPLIER foreign key (Supplier_Company)
      references Supplier (Supplier_Company)
      on delete restrict on update restrict;

alter table HistotySupply
   add constraint FK_HISTOTYS_HAVE2_SUPPLIER foreign key (Supplier_Company)
      references Supplier (Supplier_Company)
      on delete restrict on update restrict;

alter table HistotySupply
   add constraint FK_HISTOTYS_HAVE3_REPAIRPA foreign key (RepairParts_ID)
      references RepairParts (RepairParts_ID)
      on delete restrict on update restrict;

alter table "Order"
   add constraint FK_ORDER_CREATE_CLIENT foreign key (Client_ID)
      references Client (Client_ID)
      on delete restrict on update restrict;

alter table "Order"
   add constraint FK_ORDER_INCLUDES_TECHNIC foreign key (Technic_ID)
      references Technic (Technic_ID)
      on delete restrict on update restrict;

alter table "Order"
   add constraint FK_ORDER_REGISTER_STAFF foreign key (Staff_ID)
      references Staff (Staff_ID)
      on delete restrict on update restrict;

alter table Uses
   add constraint FK_USES_USES_ORDER foreign key (Order_ID)
      references "Order" (Order_ID)
      on delete restrict on update restrict;

alter table Uses
   add constraint FK_USES_USES2_REPAIRPA foreign key (RepairParts_ID)
      references RepairParts (RepairParts_ID)
      on delete restrict on update restrict;
 
