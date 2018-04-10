using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace ShopBot.Services
{
    public class FluentSearchClient
    {
        private readonly ISearchIndexClient _searchIndexClient;
        private readonly SearchParameters _searchParameters;

        private FluentSearchClient(ISearchIndexClient searchIndexClient)
        {
            _searchIndexClient = searchIndexClient;
            _searchParameters = new SearchParameters();
        }

        public List<T> Find<T>(string key) where T : class
        {
            return _searchIndexClient.Documents
                .Search<T>(key, _searchParameters, new SearchRequestOptions { })
                .Results.Select(s => s.Document)
                .ToList();
        }

        public FluentSearchClient Select(string field)
        {
            (_searchParameters.Select ?? (_searchParameters.Select = new List<string>())).Add(field);
            return this;
        }
        
        public FluentSearchClient Limit(int number)
        {
            _searchParameters.Top = number;
            return this;
        }

        public SortedFluentSearchClient Sort(string field)
        {
            return new SortedFluentSearchClient(this, field);
        }
        
        public FluentSearchClient Sort(string field, string order)
        {
             (_searchParameters.OrderBy ??
                    (_searchParameters.OrderBy = new List<string>())).Add($"{field} {order}");
            return this;
        }

        public class SortedFluentSearchClient
        {
            private readonly FluentSearchClient _fluentSearchClient;
            private readonly string _field;

            public SortedFluentSearchClient(FluentSearchClient fluentSearchClient, string field)
            {
                _fluentSearchClient = fluentSearchClient;
                _field = field;
            }

            public FluentSearchClient Ascending()
            {
                (_fluentSearchClient._searchParameters.OrderBy ??
                 (_fluentSearchClient._searchParameters.OrderBy = new List<string>())).Add($"{_field} asc");
                return _fluentSearchClient;
            }

            public FluentSearchClient Descending()
            {
                (_fluentSearchClient._searchParameters.OrderBy ??
                 (_fluentSearchClient._searchParameters.OrderBy = new List<string>())).Add($"{_field} desc");
                return _fluentSearchClient;
            }
        }

        public static FluentSearchClient Of(ISearchIndexClient searchIndexClient)
        {
            return new FluentSearchClient(searchIndexClient);
        }
    }
}