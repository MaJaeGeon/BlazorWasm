using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using Microsoft.AspNetCore.Components;

namespace  BlazorWasm.Pages {
    public partial class Index : ComponentBase {
        [Parameter]
        public string? link { get; set; }
        protected List<Entry>? Items;

        protected override async Task OnParametersSetAsync()
        {
            GraphQLHttpClient graphQLClient = new GraphQLHttpClient("https://api.github.com/graphql", new SystemTextJsonSerializer());
                
            var personAndFilmsRequest = new GraphQLRequest {
            Query =@"
                query GetPosts($repoName: String!, $repoOwner: String!, $path: String){
                    repository(name: $repoName, owner: $repoOwner) {
                            object(expression: $path) {
                                ... on Tree {
                                entries {
                                    name
                                    path
                                }
                            }
                        }
                    }
                }
            ",
            Variables = new {
                repoName = "majaegeon.github.io",
                repoOwner = "MaJaeGeon",
                path = $"HEAD:posts/{link}"
            }};
        
            graphQLClient.HttpClient.DefaultRequestHeaders.Add("Authorization", $"bearer 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");
            var graphqlResponse = await graphQLClient.SendQueryAsync<Posts>(personAndFilmsRequest);

            Items = graphqlResponse.Data.Repository.Object.Entries;
        }


        public class Posts
        {
            public Repository Repository { get; set; }
        }

        public class Entry
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }

        public class Object
        {
            public List<Entry> Entries { get; set; }
        }

        public class Repository
        {
            public Object Object { get; set; }
        }
    }
}