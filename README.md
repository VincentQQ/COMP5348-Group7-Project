# COMP5348-Group7-Project
COMP5348-Group-Project-Integrating-Enterprise-Applications

# How to run
The following instruction will describe how to run this project using Visual Studio 2019.

## Requirements
MSMQ are required to run this project. Enable MSMQ by going to "Turn Windows features on or off" and checking the box for MSMQ.

## Creating the database
Before running the code, the database for BookStore, Bank, and DeliveryCo needs to be created.
1. Run an SQLQuery to create database BookStore, Bank, and DeliveryCo in (localdb)/MSSQLLocalDB.
2. Run the following sql queries on BookStore, Bank, and DeliveryCo respectively
    * BookStore.Entities\BookStore.Business.Entities\BookStoreEntityModel.edmx.sql
    * Bank.Business\Bank.Business.Entities\BankEntityModel.edmx.sql
    * DeliveryCo.Business\DeliveryCo.Business.Entities\DeliveryCoEntityModel.edmx.sql

## Running the project
 
1. Run the Visual Studio 2019 as administrator
2. Run the following (In Visual 2019, right-click on each file, select Debug -> Start New Instance)
    * Run MessageBus
    * Bank.Application\Bank.Process
    * BookStore.Application\BookStore.Process
    * DeliveryCo.Application\DeliveryCo.Process ***twice to get a backup***
    * EmailService.Application\EmailService.Process
3. Run BookStore.Presentation\BookStore.WebClient

# Assumptions
The folowing assumptions were made regarding the requirement of this project:
* Customer will make the delivery request via the BookStore application. BookStore sends the request to DeliveryCo to be processed.
* When one warehouse is not enough to fulfil the order, BookStore will find a minimum set of warehouses that can handle the order. One book can be split across multiple warehouses.
