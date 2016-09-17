var React = require('react');

module.exports = React.createClass({

    render: function () {
        var model = this.props.model;
        var actions = this.props.actions;
        var that = this;
        return (<div>
                <br />
                    <h5>{model.squad.length > 0 ? "Tropp" :""}</h5>
    <div className="flex flex-justify-center game-squadList">
                <ul className="list-unstyled">
                    {model.squad.map(function (player) {
                        var showPlayerUrl = model.showPlayerUrl + '/' + player.urlName;
                        return (<li key={player.id}>
                                    <i className="flaticon-soccer18"></i>&nbsp;<a href={showPlayerUrl} className="underline">{player.fullName}</a>
                                    {
                                        model.isRemovingPlayer == player.id ?
                                        <i className="fa fa-spinner fa-spin removePlayer"></i>:
                                        that.renderDeleteButton(player, actions, model)
                                    }
                                </li>);
})}
                    <li key="loader">{model.isAddingPlayer ? <i className="fa fa-spinner fa-spin"></i> : ''}</li>
                </ul>

    </div>
    </div>
    )
    },
    renderDeleteButton: function (player, actions, model) {
        if (model.editMode != false) {
            return (<a className="pull-right" onClick={actions.removePlayerFromSquad.bind(null, player.id) }>&nbsp;&nbsp;<i className="text-danger fa fa-times"></i></a>)

            }
}

});
