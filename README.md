# YWebAPI
Personal PI Web API project using AF SDK

The project is to create an ASP.Net Web API application that enabled the data reads and writes into a PI System

The project currently has 1 functional controllers and 3 actions:

tsopsController
    1.Get request to obtain a snapshot value by providing the pointID in the URL 
        URL format api/tsops/ptid
    2.Put request to place a snapshot value (provided as a string in the body of the request) into a pointID specified in the URL
        URL format api/tsops/ptid
        request body example: "123"
    3.Post request to create a PI Point with a tag name specified in the body of the request
        URL format: api/tsops
        request body example: body "testtag"
        
