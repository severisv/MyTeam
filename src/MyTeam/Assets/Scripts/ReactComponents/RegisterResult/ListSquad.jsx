var ListSquad = React.createClass({

    render: function () {
        var model = this.props.model;
        var actions = this.props.actions;
        var that = this;
        return (<div>
                <br />
                    <h5>Tropp</h5>
    <div className="flex flex-justify-center game-squadList">
                <ul className="list-unstyled">
                    {model.squad.map(function (player) {
                        var showPlayerUrl = model.showPlayerUrl + '?playerId='+player.Id
                        return(<li key={player.Id}>
                                    <i className="flaticon-soccer18"></i>&nbsp;<a href={showPlayerUrl} className="underline">{player.FullName}</a>
                                    {that.renderDeleteButton(player, actions, model)}
                                </li>)
})}
                </ul>

    </div>
    </div>
    )
    },
    renderDeleteButton: function (player, actions, model) {
        if (model.editMode != false) {
            return (<a className="pull-right" onClick={actions.removePlayerFromSquad.bind(null, player.Id) }>&nbsp;&nbsp;<i className="text-danger fa fa-times"></i></a>)

            }
}
    
});
