var ListSquad = React.createClass({

    render: function () {
        var model = this.props.model;
        var actions = this.props.actions;
        return (<div>
                <br />
                    <h4>Tropp</h4>
    <div className="flex flex-justify-center game-squadList">
                <ul className="list-unstyled">
                    {model.squad.map(function(player) {
                        return(<li key={player.Id}>
                                    <i className="flaticon-soccer18"></i>&nbsp;<a className="underline">{player.FullName}</a>
                                </li>)
})}
                </ul>

    </div>
    </div>
    )
}
});
