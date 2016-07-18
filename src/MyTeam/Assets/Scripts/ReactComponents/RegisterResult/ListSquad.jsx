﻿var React = require('react');

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
                        var showPlayerUrl = model.showPlayerUrl + '/'+player.id
                        return(<li key={player.id}>
                                    <i className="flaticon-soccer18"></i>&nbsp;<a href={showPlayerUrl} className="underline">{player.fullName}</a>
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
            return (<a className="pull-right" onClick={actions.removePlayerFromSquad.bind(null, player.id) }>&nbsp;&nbsp;<i className="text-danger fa fa-times"></i></a>)

            }
}

});
