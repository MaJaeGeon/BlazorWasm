using GraphQL;
using GraphQL.Client.Http;
using Microsoft.AspNetCore.Components;
using GraphQL.Client.Serializer.SystemTextJson;

namespace BlazorWasm.Pages
{
    public partial class Index : ComponentBase{
    protected List<Entry>? Items;

        protected override async Task OnInitializedAsync()
        {
            GraphQLHttpClient graphQLClient = new GraphQLHttpClient("https://api.example.com/graphql", new SystemTextJsonSerializer());

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
                }",
                Variables = new {
                    repoName = "majaegeon.github.io",
                    repoOwner = "MaJaeGeon",
                    path = $"HEAD:posts/{link}"
                }
            };


            var graphqlResponse = await graphQLClient.SendQueryAsync<Posts>(personAndFilmsRequest);
            Items = graphqlResponse.Data.Data.Repository.Object.Entries;
        }

        [Parameter]
        public string? link { get; set; }
    }

    public class Data
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

    public class Posts
    {
        public Data Data { get; set; }
    }

}