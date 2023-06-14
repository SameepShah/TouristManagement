/* GIT URLs */

Backend: https://github.com/SameepShah/TouristManagement
Frontend: https://github.com/SameepShah/touristmanagement-ui

Backend Prerequisites
----------------------

•	Visual Studio 2022 Community Edition
•	Visual Studio Code
•	NodeJS (Latest Stable Version)
•	Azure Cosmos DB Emulator
•	Docker Desktop
	o	RabbitMQ (Image)
	o	Redis-local (Image)
	o	Elasticsearch (Image)
	o	Kibana (Image)
•	Consul (consul_1.15.2_windows_386)
•	Ocelot API Gateway
•	Postman
•	Gitbash

Backend Configurations
--------------------------------

1) Azure Cosmos DB Emulator
•	Download Azure Cosmos DB Emulator
•	Create database “touristmgmtdb”
•	Create following collections under “touristmgmtdb”
	o	“Branches” - /BranchCode (UniqueId, PartitionKey)
	o	“Places” - /PlaceId (PartitionKey)
	o	“Users” - /UserId (PartitionKey)
•	Format of the collections will be added in separate JSON files.
•	Appsettings Configuration for Cosmos DB Configuration in AdminAPI and BranchAPI

	"AzureCosmosDbSettings": {
		 "URL": "<<CosmosDBURL>>",
		 "PrimaryKey": "<<PrimaryKeyFromCosmosDB>>",
		 "DatabaseName": "touristmgmtdb"
	} 

2) RabbitMQ

•	Pull RabbitMQ Docker Image: docker pull rabbitmq:3-management
•	Run RabbitMQ Container: docker run -d -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
•	Localhost URL for RabbitMQ: http://localhost:15672/
•	Default Username/Password: guest/guest
•	RabbitMQ Appsetting configuration in AdminAPI
	"MessageCosumerSettings": {
		"HostName": "localhost",
		"UserName": "guest",
		"Password": "guest"
	}


3) Consul

•	Download Consul (consul_1.15.2_windows_386)
•	Run Consul in localhost (localhost:8500) using following command in command prompt
	o	consul agent –dev
•	Consul appsetting configuration for AdminAPI and BranchAPI
	"ConsulConfig": {
		"Host": "http://localhost:8500",
		"ServiceName": "branchservice",
		"ServiceId": "branchservice-in1",
		"DefaultServerAddress": https://localhost:7276 //URL on which API Runs
	}

4) Redis Cache

•	Pull Redis Docker Image: docker pull redis
•	Run docker container:  docker run --name redis-local -p 5002:6379 -d redis
•	Redis Cache Appsetting Configuration in AdminAPI
	"RedisCache": {
		"ConnectionString":"localhost:5002" //Redis ConnectionString from Docker Image or Azure
	}


5) ELK Stack

•	Run Docker Compose command from the place of the docker compose file.
	o	docker-compose up -d
•	Port for the Elastic Search Container http://localhost:9200
•	Port for the Kibana Container:  http://localhost:5601
•	ELK Appsetting configuration in AdminAPI and BranchAPI

	"ElasticConfiguration": {
		"Uri": "http://localhost:9200"
	}


Backend API Endpoints
-------------------------------------------------------------
1.	Authenticate
URL: /tourism/api/v1/account/authenticate
Method: POST
Payload:
{
	“UserName”:”user1”,
	“Password”:”user@1”
}


2.	Search
URL: /tourism/api/v1/admin/search
Method: POST
Payload:
{
    "id":"",
    "BranchCode":"AHD-001",
    "BranchName":"",
    "Place":"thailand",
    "PaginationSorting":{
        "PageIndex":1,
        "PageSize":10,
        "SortColumn":"BranchName",
        "SortOrder": false
    }
}


3.	Add Branch
URL: /tourism/api/v1/branch/addbranch
Method: POST
Payload

{
    "BranchName": "MUMBAI BRANCH 2",
    "BranchCode": "MUM-002",
    "Website": "www.tourism.com",
    "Contact": "12345645",
    "Email": "mum02@tourism.com",
    "Places": [
        {
            "PlaceId": "1",
            "PlaceName": "ANDAMAN",
            "TariffAmount": 60000
        },
        {
            "PlaceId": "2",
            "PlaceName": "THAILAND",
            "TariffAmount": 50000
        },
        {
            "PlaceId": "3",
            "PlaceName": "DUBAI",
            "TariffAmount": 70000
        },
        {
            "PlaceId": "4",
            "PlaceName": "SINGAPORE",
            "TariffAmount": 90000
        },
        {
            "PlaceId": "5",
            "PlaceName": "MALAYSIA",
            "TariffAmount": 75000
        }
    ]
}


4.	Update Branch
URL: /tourism/api/v1/branch/editbranch
Method: POST
Payload:
{
    "Id":"cf65253a-aa5c-4c88-9162-ce40cbf01c45",
    "BranchCode": "AHD-001",
    "Places": [
        {
            "PlaceId": "1",
            "PlaceName": "ANDAMAN",
            "TariffAmount": 60000
        },
        {
            "PlaceId": "2",
            "PlaceName": "THAILAND",
            "TariffAmount": 50000
        },
        {
            "PlaceId": "3",
            "PlaceName": "DUBAI",
            "TariffAmount": 70000
        },
        {
            "PlaceId": "4",
            "PlaceName": "SINGAPORE",
            "TariffAmount": 90000
        },
        {
            "PlaceId": "5",
            "PlaceName": "MALAYSIA",
            "TariffAmount": 75000
        }
    ]
}


