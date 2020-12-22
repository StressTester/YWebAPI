using OSIsoft.AF.PI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OSIsoft.AF.Asset;

namespace YWebAPI.Controllers
{
    public class tsopsController : ApiController
    {

        // GET snapshot value using point ID
        // api/tsops/id
        // example url https://localhost:44375/api/tsops/3
        public IHttpActionResult GetSnapshot(int id)
        {
            //// Get the PIServers collection for the current user and default PIServer
            PIServer myPIServer = new PIServers().DefaultPIServer;

            //instantiate varaibles and initialize PIPoint object 
            PIPoint selectedPoint = PIPoint.FindPIPoint(myPIServer, id);
            string snapshotVal;
            string snapshotTime;

            //cast extracted value to string if tag exists
            try
            {
                snapshotVal = selectedPoint.CurrentValue().Value.ToString();
                snapshotTime = selectedPoint.CurrentValue().Timestamp.ToString();
            }
            catch (Exception e)
            {
                //400 error code with failure information
                //Expected possible OSIsoft.AF.PI.PIPointInvalidException
                return BadRequest($"PI point with pointID {id} was not found on server {myPIServer.Name}: " + e.Message.ToString());
            }
            //200 status return with snapshot value of requested tag
            return Ok($"the snapshot value of the PI Point \"{selectedPoint.Name}\" is {snapshotVal} {snapshotTime}");
        }


        // Put snapshot value using point ID
        // api/tsops/id
        // example url https://localhost:44375/api/tsops/3
        // Example body "123"
        public IHttpActionResult PutSnapshot(int id, [FromBody]string value)
        {

            //// Get the PIServers collection for the current user and default PIServer
            PIServer myPIServer = new PIServers().DefaultPIServer;

            //instantiate and initialize varaibles and initialize PIPoint object 
            PIPoint selectedPoint = PIPoint.FindPIPoint(myPIServer, id);
            string snapshotVal = value;
            DateTime snapshotTime = DateTime.Now;
            AFValue snapAFVal = new AFValue(value);
            try
            {
                //Insert value at current time
                selectedPoint.UpdateValue(snapAFVal, OSIsoft.AF.Data.AFUpdateOption.Insert);
            }
            catch (Exception e)
            {
                //TODO Could split exceptions to multiple catch blocks to make error message more specific
                //400 error code with failure information
                //Expected incorrect data type errors and tag not found errors 
                return BadRequest($"PI point with pointID {id} was not found on server {myPIServer.Name} or " +
                    $"value provided had incorrect data type: " + e.Message.ToString());
            }
            //200 status return posted value
            return Ok($"{value} inserted at {snapshotTime} for tag {selectedPoint.Name}");
        }


        // Post create tag with tag name
        // api/tsops
        // example url https://localhost:44375/api/tsops
        // Example body "testtag"
        // TODO enable setting more parameters to create tag such as point type etc
        public IHttpActionResult PostPITag([FromBody] string tagName)
        {

            //// Get the PIServers collection for the current user and default PIServer
            PIServer myPIServer = new PIServers().DefaultPIServer;

            // Attempt to create PI point with provided name
            try
            {
                myPIServer.CreatePIPoint(tagName);
            }
            catch (Exception e)
            {
                //400 error code with failure information
                //Expected tag already exists, invalid name, point limit reached, server not available
                return BadRequest($"Failed to create PI Point with name {tagName} on the server {myPIServer.Name}: " + e.Message.ToString());
            }
            //200 status return with message containing created tag name
            return Ok($"{tagName} created on {myPIServer.Name}");
        }

    }
}
