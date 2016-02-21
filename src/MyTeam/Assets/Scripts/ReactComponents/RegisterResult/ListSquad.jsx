var ListSquad = React.createClass({

    render: function () {
        var model = this.props.model;
        var actions = this.props.actions;
        return (<div>
                <br />
                    <h5>Tropp</h5>
    <div className="flex flex-justify-center game-squadList">
                <ul className="list-unstyled">
                    {model.squad.map(function (player) {
                        var showPlayerUrl = model.showPlayerUrl + '?playerId='+player.Id
                        return(<li key={player.Id}>
                                    <i className="flaticon-soccer18"></i>&nbsp;<a href={showPlayerUrl} className="underline">{player.FullName}</a>
                                </li>)
})}
                </ul>

    </div>
    </div>
    )
}
});
