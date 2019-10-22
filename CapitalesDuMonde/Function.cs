using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Alexa.NET.Request;
using Alexa.NET.Response;
using Amazon;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace CapitalesDuMonde
{
    public class Function
    {
        //Constantes 
        private const string BUCKET_NAME = "listePaysfr";
        private const string KEY_NAME = "monde.xml";
        private const string SESSION_STATE_KEY = "skill-state";

        private static readonly RegionEndpoint BucketRegion = RegionEndpoint.EUWest1;
        public static IAmazonS3 S3Client;
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<SkillResponse> FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            S3Client = new AmazonS3Client(BucketRegion);
            MemoryStream memory = new MemoryStream();
            //Construct response
            var response = new SkillResponse
            {
                Response = new ResponseBody()
            };
            response.Response.ShouldEndSession = false;
            IOutputSpeech innerResponse = null;
            string reponse = "";

            //R�cup�rer la session
            var playerSession = input.Session;
            int playerScore = RestoreState(playerSession);

            //R�cup�rer le fichier xml 
            GetS3File(S3Client, memory);

            var monde = new Monde();
            var serialize = new XmlSerializer(typeof(Monde));
            monde = (Monde)serialize.Deserialize(memory);

            foreach(var m in monde.Continent)
            {
                LambdaLogger.Log(m.Nom);
            }

            innerResponse = new SsmlOutputSpeech();
            ((SsmlOutputSpeech)innerResponse).Ssml = $"<speak>{reponse}</speak>";
            response.Version = "1.0";
            response.Response.OutputSpeech = innerResponse;
            response.SessionAttributes = new Dictionary<string, object> { [SESSION_STATE_KEY] = playerScore };

            return response;
        }

        /// <summary>
        /// R�cup�rer le score stoquer dans la sessions
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public int RestoreState(Session session)
        {
            int score = 0;
            if (session != null && session.New)
            {
                LambdaLogger.Log("New Session");
            }
            else
            {
                LambdaLogger.Log("Exesting Session");
                //R�cup�rer l'objet stock� en session
                var playerStateValue = session.Attributes.GetValueOrDefault(SESSION_STATE_KEY) ?? null;
                try
                {
                    score = (int)playerStateValue;
                }
                catch (Exception)
                {
                    LambdaLogger.Log("Converstion Exception");
                    return 0;
                }
            }
            return score;

        }

        /// <summary>
        /// r�cuperer le fichier stocker dans S3
        /// </summary>
        /// <param name="S3Client"></param>
        public async void GetS3File(IAmazonS3 S3Client, MemoryStream memory)
        {
            var request = new GetObjectRequest
            {
                BucketName = BUCKET_NAME,
                Key = KEY_NAME
            };

            try
            {
                LambdaLogger.Log("Get S3 file : ");
                using (var s3Response = await S3Client.GetObjectAsync(request))
                {
                   // var memory = new MemoryStream();
                    await s3Response.ResponseStream.CopyToAsync(memory);
                }
            }
            catch (AmazonS3Exception e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                LambdaLogger.Log($"Error encountered ***. Message:'{0}' when writing an object {e.Message}");
                throw new Exception("unable to load file from s3");
            }
            catch (Exception e)
            {
                LambdaLogger.Log($"Unknown encountered on server. Message:'{0}' when writing an object {e.Message}");
                throw new Exception("Unknown encountered on server");
            }

        }

    }

}

