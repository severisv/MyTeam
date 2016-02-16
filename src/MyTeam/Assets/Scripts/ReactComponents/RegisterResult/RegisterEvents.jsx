var RegisterEvents = React.createClass({
    getInitialState: function () {
        return ({
            players: [],
            eventTypes: []
        });
    },


    componentDidMount: function(){
        var that = this;
        $.getJSON(that.props.routes.GET_PLAYERS).then(function (response) {
            console.log(response);

            that.setState({
                players: response
            });
        });
        $.getJSON(that.props.routes.GET_EVENTTYPES).then(function (response) {

            that.setState({
                eventTypes: response
            });
        });
    },

    actions: {
        
    },
  

    render: function () {

        return(<RegisterEventsView model={this.state} actions={this.actions}></RegisterEventsView>);
    }
       
        
    
});
