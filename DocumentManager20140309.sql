CREATE DATABASE  IF NOT EXISTS `documentmanager` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `documentmanager`;
-- MySQL dump 10.13  Distrib 5.6.11, for Win32 (x86)
--
-- Host: localhost    Database: documentmanager
-- ------------------------------------------------------
-- Server version	5.5.33

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `action`
--

DROP TABLE IF EXISTS `action`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `action` (
  `action_id` int(11) NOT NULL AUTO_INCREMENT,
  `action_name` varchar(45) DEFAULT NULL,
  `supper_action_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`action_id`)
) ENGINE=InnoDB AUTO_INCREMENT=10000112 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `action`
--

LOCK TABLES `action` WRITE;
/*!40000 ALTER TABLE `action` DISABLE KEYS */;
INSERT INTO `action` VALUES (100001,'档案管理',NULL),(100002,'档案查询',NULL),(100003,'代开台账',NULL),(100004,'系统管理',NULL),(1000011,'建安代开档案',100001),(1000012,'普票代开档案',100001),(1000013,'专票代开档案',100001),(1000014,'固定户档案',100001),(1000021,'固定户档案查询',100002),(1000022,'建安档案查询',100002),(1000023,'普票查询',100002),(1000024,'专票查询',100002),(1000041,'人员管理',100004),(1000042,'人员权限设置',100004),(1000043,'档案类型',100004),(1000044,'注册类型',100004),(1000045,'删除文档',100004);
/*!40000 ALTER TABLE `action` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `filetype`
--

DROP TABLE IF EXISTS `filetype`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `filetype` (
  `file_type_id` int(11) NOT NULL AUTO_INCREMENT,
  `file_type_name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`file_type_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `filetype`
--

LOCK TABLES `filetype` WRITE;
/*!40000 ALTER TABLE `filetype` DISABLE KEYS */;
INSERT INTO `filetype` VALUES (1,'登记证'),(2,'拨款报告'),(3,'申报表');
/*!40000 ALTER TABLE `filetype` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `systemlog`
--

DROP TABLE IF EXISTS `systemlog`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `systemlog` (
  `system_log_id` bigint(20) NOT NULL AUTO_INCREMENT,
  `system_log` text,
  PRIMARY KEY (`system_log_id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `systemlog`
--

LOCK TABLES `systemlog` WRITE;
/*!40000 ALTER TABLE `systemlog` DISABLE KEYS */;
/*!40000 ALTER TABLE `systemlog` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `taxpayer`
--

DROP TABLE IF EXISTS `taxpayer`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `taxpayer` (
  `taxpayer_id` int(11) NOT NULL AUTO_INCREMENT,
  `taxpayer_code` varchar(400) DEFAULT NULL,
  `taxpayer_name` varchar(400) DEFAULT NULL,
  `taxpayer_type_id` int(11) DEFAULT NULL,
  `taxpayer_group_id` int(11) DEFAULT NULL,
  `taxpayer_regyear` varchar(45) DEFAULT NULL,
  `taxpayer_project` varchar(400) DEFAULT NULL,
  `taxpayer_project_finish` bit(1) DEFAULT NULL,
  `taxpayer_res1` varchar(400) DEFAULT NULL,
  `taxpayer_res2` varchar(400) DEFAULT NULL,
  `taxpayer_res3` varchar(400) DEFAULT NULL,
  `taxpayer_res4` varchar(400) DEFAULT NULL,
  PRIMARY KEY (`taxpayer_id`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `taxpayer`
--

LOCK TABLES `taxpayer` WRITE;
/*!40000 ALTER TABLE `taxpayer` DISABLE KEYS */;
/*!40000 ALTER TABLE `taxpayer` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `taxpayerdocument`
--

DROP TABLE IF EXISTS `taxpayerdocument`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `taxpayerdocument` (
  `taxpayer_document_id` int(11) NOT NULL AUTO_INCREMENT,
  `taxpayer_id` int(11) DEFAULT NULL,
  `taxpayer_document_name` varchar(400) DEFAULT NULL,
  `taxpayer_document_type_id` int(11) DEFAULT NULL,
  `taxpayer_document_descript` varchar(400) DEFAULT NULL,
  `taxpayer_update_user_id` int(11) DEFAULT NULL,
  `taxpayer_update_time` datetime DEFAULT NULL,
  `taxpayer_document_bytes` bigint(20) DEFAULT NULL,
  `taxpayer_document_res1` varchar(400) DEFAULT NULL,
  `taxpayer_document_res2` varchar(400) DEFAULT NULL,
  `taxpayer_document_res3` varchar(400) DEFAULT NULL,
  `taxpayer_document_res4` varchar(400) DEFAULT NULL,
  PRIMARY KEY (`taxpayer_document_id`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `taxpayerdocument`
--

LOCK TABLES `taxpayerdocument` WRITE;
/*!40000 ALTER TABLE `taxpayerdocument` DISABLE KEYS */;
/*!40000 ALTER TABLE `taxpayerdocument` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `taxpayertype`
--

DROP TABLE IF EXISTS `taxpayertype`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `taxpayertype` (
  `taxpayertype_id` int(11) NOT NULL AUTO_INCREMENT,
  `taxpayertype_name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`taxpayertype_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `taxpayertype`
--

LOCK TABLES `taxpayertype` WRITE;
/*!40000 ALTER TABLE `taxpayertype` DISABLE KEYS */;
INSERT INTO `taxpayertype` VALUES (1,'个体'),(2,'股份'),(3,'国有');
/*!40000 ALTER TABLE `taxpayertype` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user`
--

DROP TABLE IF EXISTS `user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `user` (
  `user_id` int(11) NOT NULL AUTO_INCREMENT,
  `user_name` varchar(45) DEFAULT NULL,
  `user_password` varchar(45) DEFAULT NULL,
  `user_cname` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user`
--

LOCK TABLES `user` WRITE;
/*!40000 ALTER TABLE `user` DISABLE KEYS */;
INSERT INTO `user` VALUES (1,'admin','e10adc3949ba59abbe56e057f20f883e','管理员');
/*!40000 ALTER TABLE `user` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `useraction`
--

DROP TABLE IF EXISTS `useraction`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `useraction` (
  `user_action_id` int(11) NOT NULL AUTO_INCREMENT,
  `user_id` int(11) DEFAULT NULL,
  `action_id` int(11) DEFAULT NULL,
  `hasRight` bit(1) DEFAULT NULL,
  PRIMARY KEY (`user_action_id`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `useraction`
--

LOCK TABLES `useraction` WRITE;
/*!40000 ALTER TABLE `useraction` DISABLE KEYS */;
/*!40000 ALTER TABLE `useraction` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping routines for database 'documentmanager'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2014-03-09 20:35:09
