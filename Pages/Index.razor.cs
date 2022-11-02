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
                
            var query = new GraphQLRequest {
            Query =@"
            query GetStructures($repoName: String!, $repoOwner: String!, $path: String!){
                repository(name: $repoName, owner: $repoOwner) {
                    object(expression: $path) {
                        ... on Tree {
                                entries {
                                name
                                path
                                object {
                                    ... on Tree {
                                        entries {
                                            name
                                            path
                                            object {
                                                ... on Tree {
                                                    entries {
                                                        name
                                                        path
                                                        type
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            ",
            Variables = new {
                repoName = "BlazorWasm",
                repoOwner = "MaJaeGeon",
                path = $"posts:"
            }};
        
            graphQLClient.HttpClient.DefaultRequestHeaders.Add("Authorization", $"bearer ");
            var graphqlResponse = await graphQLClient.SendQueryAsync<Root>(query);

            Items = graphqlResponse.Data.Data.Repository.Object.Entries;
            // 여기서 이제 Blob랑 Tree 랑 구분해서 루프돌려야함.
        }


        public class Data {
            public Repository Repository { get; set; } = null!;
        }

        public class Entry {
            public string Name { get; set; } = null!;
            public string Path { get; set; } = null!;
            public Object Object { get; set; } = null!;
            public string Type { get; set; } = null!;
        }

        public class Object {
            public List<Entry> Entries { get; set; } = null!;
        }

        public class Repository {
            public Object Object { get; set; } = null!;
        }

        public class Root {
            public Data Data { get; set; } = null!;
        }

    }
}
