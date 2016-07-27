using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;

namespace MyElasicSearchApp
{
    class Program
    {
        //connection config
        public static Uri node;
        public static ConnectionSettings settings;

        //ES client
        public static ElasticClient client;

        static void Main(string[] args)
        {
            //set up connection
            var node = new Uri("http://localhost:9200");
            var settings = new ConnectionSettings(node)
                .DefaultIndex("my_blog");
            
            //create client instance
            client = new ElasticClient(settings);

            //check if index exists
            var resolver = new IndexNameResolver(settings);
            var index = resolver.Resolve<Post>();

            if (index == "") {
                //create new index
                client.CreateIndex("my_blog", c => c
                 .Settings(s => s
                 .NumberOfReplicas(1)
                 .NumberOfShards(1)
                 )
                 //map index to a class with a bunch of properties
                 .Mappings(m => m
                 .Map<Post>(p => p
                 .AutoMap())));
            }
            else {
                //call method to insert a row into existing index
                InsertData();
            }
            
        }

        public static void InsertData()
        {
            //create new instance of Post and assign values
            var newBogPost = new Post
            {
                UserID =2,
                PostDate = DateTime.Now,
                PostText = "This is my second blog post!"
            };
            //POST to index
            client.Index(newBogPost);
        }

    }
}
