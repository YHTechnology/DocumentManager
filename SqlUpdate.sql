insert into action (action_id, action_name, supper_action_id) values (1000031, '建安代开', 100003);
insert into action (action_id, action_name, supper_action_id) values (1000032, '普票代开', 100003);
insert into action (action_id, action_name, supper_action_id) values (1000033, '专票代开', 100003);


CREATE TABLE `documentmanager`.`standbook` (
  `standbook_id` VARCHAR(45) NOT NULL,
  `projectname` VARCHAR(45) NULL,
  `totalmoney` DECIMAL(20,3) NULL,
  `taxpayername` VARCHAR(45) NULL,
  `taxpayerpersonname` VARCHAR(45) NULL,
  `capitalcontruction` VARCHAR(45) NULL,
  `hasoutverify` BIT NULL,
  `paytime` DATETIME NULL,
  `thispartmoney` DECIMAL(20,3) NULL,
  `hasbusinesstax` BIT NULL,
  `businesstax` DECIMAL(20,3) NULL,
  `haseducationsurtax` BIT NULL,
  `educationsurtax` DECIMAL(20,3) NULL,
  `hasurbantax` BIT NULL,
  `urbantax` DECIMAL(20,3) NULL,
  `haslocaleducationsurtax` BIT NULL,
  `localeducationsurtax` DECIMAL(20,3) NULL,
  `hasstamptax` BIT NULL,
  `stamptax` DECIMAL(20,3) NULL,
  `hasincometax` BIT NULL,
  `incometax` DECIMAL(20,3) NULL,
  `invoicenumber` VARCHAR(45) NULL,
  `taxreceiptnumber` VARCHAR(45) NULL,
  `groupid` INT NULL,
  PRIMARY KEY (`standbook_id`));

