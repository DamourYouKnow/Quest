using System;

namespace Quest.Core.Events {
    interface IEvent {
        void RunEvent();
    }

    public class ChivalrousDeedEvent : IEvent {
        public void RunEvent() {
            throw new NotImplementedException();
        }
    }

    public class CourtCalledEvent : IEvent {
        public void RunEvent() {
            throw new NotImplementedException();
        }
    }

    public class CallToArmsEvent : IEvent {
        public void RunEvent() {
            throw new NotImplementedException();
        }
    }

    public class RecognitionEvent : IEvent {
        public void RunEvent() {
            throw new NotImplementedException();
        }
    }

    public class PlagueEvent : IEvent {
        public void RunEvent() {
            throw new NotImplementedException();
        }
    }

    public class PoxEvent : IEvent {
        public void RunEvent() {
            throw new NotImplementedException();
        }
    }

    public class ProsperityEvent : IEvent {
        public void RunEvent() {
            throw new NotImplementedException();
        }
    }
    
}
