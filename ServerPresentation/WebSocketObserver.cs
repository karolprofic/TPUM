using Commons;

namespace ServerPresentation
{
    internal class WebSocketObserver : IObserver<List<CandidateDTO>>
    {
        private readonly WebSocketConnection _client;
        private readonly JsonSerializer _serializer;

        public WebSocketObserver(WebSocketConnection client, JsonSerializer serializer)
        {
            _client = client;
            _serializer = serializer;
        }

        public void OnNext(List<CandidateDTO> data)
        {
            var msg = new CandidatesMessage { Action = "SendCandidates", Candidates = data };
            _client.SendMessageAsync(_serializer.Serialize(msg)).Wait();
        }
        public void OnError(Exception error) { }
        public void OnCompleted() { }
    }
}
