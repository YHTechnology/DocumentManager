ALTER TABLE `documentmanager`.`taxpayerdocument` 
ADD COLUMN `taxpayer_islink` BIT NULL AFTER `taxpayer_document_res4`,
ADD COLUMN `taxpayer_linkid` INT(11) NULL AFTER `taxpayer_islink`;
ALTER TABLE `documentmanager`.`taxpayer` 
ADD COLUMN `taxpayer_isfree` BIT NULL AFTER `taxpayer_res4`,
ADD COLUMN `taxpayer_ftk` BIT NULL AFTER `taxpayer_isfree`;