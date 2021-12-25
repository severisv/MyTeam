const React = require("react");

module.exports = React.createClass({
  getIconClassName(type) {
    if (type == "Mål") return "fa fa-soccer-ball-o";
    if (type == "Gult kort") return "icon icon-card-yellow";
    if (type == "Rødt kort") return "icon icon-card-red";
  },

  render() {
    const model = this.props.model;
    const actions = this.props.actions;
    const that = this;
    const iconClassName = this.getIconClassName;

    return (
      <div className="col-sm-offset-1 gameEvents">
        {model.events.map((event, i) => (
          <div key={i} className="gameEvent">
            <span className="no-wrap">
              <i className={iconClassName(event.type)} />
              &nbsp;&nbsp;
              {that.renderPlayerLink(actions, event, model)}
            </span>
            <span>&nbsp; </span>
            {that.renderAssist(event, actions, model)}
            {that.renderDeleteButton(event, actions, model)}
            {model.isRemovingEvent == event.id ? (
              <i className="fa fa-spinner fa-spin removeEvent" />
            ) : (
              ""
            )}
          </div>
        ))}
        {model.isAddingEvent ? <i className="fa fa-spinner fa-spin" /> : ""}
      </div>
    );
  },

  renderPlayerLink(actions, event, model) {
    if (event.playerId) {
      const showPlayerUrl = `${model.showPlayerUrl}/${actions.getPlayerUrlName(
        event.playerId
      )}`;
      return (
        <a className="underline" href={showPlayerUrl}>
          {actions.getPlayerShortName(event.playerId)}
        </a>
      );
    }
    return <span>{actions.getPlayerName(event.playerId)}</span>;
  },

  renderAssist(event, actions, model) {
    if (event.assistedById) {
      const showPlayerUrl = `${model.showPlayerUrl}/${actions.getPlayerUrlName(
        event.assistedById
      )}`;
      return (
        <span className="no-wrap">
          ( <i className="flaticon-football119" />{" "}
          <a className="underline" href={showPlayerUrl}>
            {actions.getPlayerShortName(event.assistedById)}
          </a>
          )
        </span>
      );
    }
  },
  renderDeleteButton(event, actions, model) {
    if (model.editMode != false) {
      return (
        <a
          className="pull-right"
          onClick={actions.deleteEvent.bind(null, event.id)}
        >
          <i className="text-danger fa fa-times" />
        </a>
      );
    }
  },
});
