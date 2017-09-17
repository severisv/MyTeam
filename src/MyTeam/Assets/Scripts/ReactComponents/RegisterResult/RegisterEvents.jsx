var React = require("react");

module.exports = React.createClass({
  render: function() {
    var model = this.props.model;
    var actions = this.props.actions;
    var assistPlayers = [{ id: "ingen", fullName: "( Ingen )" }].concat(
      model.squad.filter(function(element) {
        return element.id != model.playerId;
      })
    );
    return (
      <div>
        <br />

        <div className="form-horizontal clearfix">
          <div className="row">
            <div className="col-xs-12">
              <div className="col-sm-12 form-group no-padding">
                <label className="col-sm-3 control-label">Hendelse</label>
                <div className="col-sm-9">
                  <select
                    className="form-control"
                    onChange={actions.handleEventChange}
                  >
                    {model.eventTypes.map(function(type) {
                      return (
                        <option key={type.value} value={type.value}>
                          {type.name}
                        </option>
                      );
                    })}
                  </select>
                  <span className="text-danger" />
                </div>
              </div>
              <div className="col-sm-12 form-group no-padding">
                <label className="col-sm-3 control-label">Hvem</label>
                <div className="col-sm-9">
                  <select
                    className="form-control"
                    onChange={actions.handlePlayerChange}
                  >
                    {actions.getEventPlayers(model.type).map(function(player) {
                      return (
                        <option key={player.id} value={player.id}>
                          {player.fullName}
                        </option>
                      );
                    })}
                  </select>
                  <span className="text-danger" />
                </div>
              </div>
              {this.renderAssistForm(model, actions, assistPlayers)}
            </div>
          </div>
          <div className="col-sm-12 form-group no-padding">
            <div className="col-sm-offset-3 col-xs-12">
              <button
                className="btn btn-primary"
                onClick={actions.handleSubmit}
              >
                Legg til
              </button>
            </div>
          </div>
        </div>
      </div>
    );
  },
  renderAssistForm: function(model, actions, assistPlayers) {
    if (model.type == 0) {
      return (
        <div className="col-sm-12 no-padding form-group">
          <label className="col-sm-3 control-label">Assist</label>
          <div className="col-sm-9">
            <select
              className="form-control"
              onChange={actions.handleAssistChange}
            >
              {assistPlayers.map(function(player) {
                return (
                  <option key={player.id} value={player.id}>
                    {player.fullName}
                  </option>
                );
              })}
            </select>
            <span className="text-danger" />
          </div>
        </div>
      );
    }
  }
});
