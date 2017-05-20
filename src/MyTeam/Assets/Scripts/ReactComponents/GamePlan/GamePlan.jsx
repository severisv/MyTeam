var React = require('react');
var PlayerInput = require('./PlayerInput.jsx')

module.exports = React.createClass({
    getInitialState: function () {
        if (this.props.gameplan) return this.props.gameplan;
        return ({
            rows: [
                {
                    time: 0,
                    lw: 'LW',
                    s: 'S',
                    rw: 'RW',
                    lcm: 'LCM',
                    rcm: 'RCM',
                    dm: 'DM',
                    lb: 'LB',
                    lcb: 'LCB',
                    rcb: 'RCB',
                    rb: 'RB',
                    gk: 'GK'
                }
            ],
            errorMessage: undefined
        });
    },

    setPlayer: function (i, key, event, input ) {

        var value = input ? input.newValue : event.target.value;

        var rows = this.state.rows;
        rows[i][key] = value;
        this.setState({
            rows: rows
        });
    },

    setTime: function (i, input) {
        var rows = this.state.rows;
        var value = parseInt(input.target.value);
        if (isNaN(value)) rows[i].time = '';
        else rows[i].time = value;
        this.setState({
            rows: rows
        });
    },

    duplicateRow: function () {
        var rows = this.state.rows;
        rows.push(JSON.parse(JSON.stringify(rows[rows.length - 1])));
        this.setState({
            rows: rows
        });
    },

    removeRow: function (i) {
        var rows = this.state.rows;
        rows.splice(i, 1);
        this.setState({
            rows: rows
        });
        this.save();
    },

    save: function () {
        var that = this;
        if (this.props.iscoach == 'True') {
            $.post('/gameapi/savegameplan', {
                gameId: this.props.gameid,
                gamePlan: JSON.stringify(that.state)
            })
            .done(function () {
                that.setState({ errorMessage: undefined });
            })
            .fail(function () {
                that.setState({ errorMessage: 'Feil ved lagring' });
            });
        }
    },

    publish: function () {
        var that = this;
        that.setState({ isPublishing: true });
        $.post('/gameapi/publishgameplan', {
            gameId: that.props.gameid
        })
            .done(function () {
                that.setState({ errorMessage: undefined, isPublishing: undefined, isPublished: true });
            })
            .fail(function () {
                that.setState({ errorMessage: 'Feil ved publisering' });
            });
    },

    renderPlayerInput(lineup, i, position) {
        var player = this.props.players.filter(function(p){ return p.Name == lineup[position]})[0];
            return(
                <div className='gp-square'>
                        {(player && player.ImageUrl) ? 
                            <img className="gameplan-playerImage" src={player.ImageUrl} /> : 
                            '' 
                        }
                        {
                            this.props.iscoach == 'True '?      
                            <PlayerInput 
                                onBlur={this.save} 
                                onChange={this.setPlayer.bind(null, i, position)} 
                                value={lineup[position]} 
                                lineup={lineup} 
                                players={this.props.players} />
                            : <input readOnly value={lineup[position]} />
                        }
                       
                </div>
            );
    },

    render: function () {
        var that = this;
        var props = this.props;

        return (
            <div className='gameplan'>
                <div className='mt-main'>
                    <div className='mt-container clearfix'>
                            <h2 className='text-center'>{props.team} vs {props.opponent}</h2>
                            <div className={this.state.errorMessage ? 'alert alert-danger' : 'hidden'}><i className='fa fa-exclamation-triangle'></i> {this.state.errorMessage}</div>
                            <br />
                            <br />
                            {this.state.rows.map(function (lineup, i) {
                                return (<div key={i}>
                                    <div className='text-center'>
                                        <input readOnly={that.props.iscoach == 'False'} className='gp-time' onBlur={that.save} onChange={that.setTime.bind(null, i)} placeholder='tid' value={lineup.time} />min
                                    </div>
                                        <button className={that.props.iscoach == 'True' && that.state.rows.length > 1 ? 'pull-right hidden-print' : 'hidden'} onBlur={that.save} onClick={that.removeRow.bind(null, i)}><i className='fa fa-times'></i></button>
                                    <br />
                                    <div className='gp-row'>
                                        {that.renderDiff(i)}
                                    </div>
                                    <br />
                                    <div className='gameplan-field'>

                                    <div className='gp-row'>
                                        {that.renderPlayerInput(lineup, i, 'lw')}
                                        <div className='gp-square'></div>
                                        {that.renderPlayerInput(lineup, i, 's')}
                                        <div className='gp-square'></div>
                                        {that.renderPlayerInput(lineup, i, 'rw')}
                                    </div>
                                    <div className='gp-row'>
                                        <div className='gp-square'></div>
                                        <div className='gp-square'></div>
                                        <div className='gp-square'></div>
                                        <div className='gp-square'></div>
                                        <div className='gp-square'></div>
                                    </div>
                                    <div className='gp-row'>
                                        <div className='gp-square'></div>
                                        {that.renderPlayerInput(lineup, i, 'lcm')}
                                        <div className='gp-square'></div>
                                        {that.renderPlayerInput(lineup, i, 'rcm')}
                                        <div className='gp-square'></div>
                                    </div>
                                    <div className='gp-row'>
                                        <div className='gp-square'></div>
                                        <div className='gp-square'></div>
                                        {that.renderPlayerInput(lineup, i, 'dm')}
                                        <div className='gp-square'></div>
                                        <div className='gp-square'></div>
                                    </div>
                                    <div className='gp-row'>
                                        {that.renderPlayerInput(lineup, i, 'lb')}
                                        {that.renderPlayerInput(lineup, i, 'lcb')}
                                        <div className='gp-square'></div>
                                        {that.renderPlayerInput(lineup, i, 'rcb')}
                                        {that.renderPlayerInput(lineup, i, 'rb')}
                                    </div>
                                    <div className='gp-row'>
                                        <div className='gp-square'></div>
                                        <div className='gp-square'></div>
                                        {that.renderPlayerInput(lineup, i, 'gk')}
                                        <div className='gp-square'></div>
                                        <div className='gp-square'></div>
                                    </div>
                                    </div>
                                    <hr />
                                </div>);
                            })}
                            <div className='text-center'>
                                <button className={that.props.iscoach == 'True' ? 'btn btn-primary' : 'hidden'} onClick={this.duplicateRow}><i className='fa fa-plus'></i></button>
                            </div>
                            <div className='clearfix'>
                                <br />&nbsp;
                            </div>
                            {this.renderPublishButton()}
                        </div>
                </div>
                <div className='mt-sidebar'>
                        <div className='mt-container'>
                            {this.renderGameTime()}
                        </div>
                </div>

            </div>

            );
    },

    renderDiff: function (i) {
        if (i < 1) return (<div></div>);
        var previous = this.state.rows[i - 1];
        var current = this.state.rows[i];


        function getPlayers(row){
            var result = [];
            for (var key in row) {
               if (key != 'time'){
                    result.push(row[key]);
                }
            }
            return result;
        }

        function isInLineup(lineup, player) {
                    var ln = [];
                    for(var key in lineup){
                            ln.push(lineup[key]);
                    }
                    return ln.indexOf(player) != -1;
        }


        function getSubs() {
            var result = [];
            for (var key in current) {
               if (key != 'time'){
                    if (previous[key] != current[key]) {                    
                        result.push({ 
                            in: !isInLineup(previous, current[key]) ? current[key] : undefined, 
                            out: !isInLineup(current, previous[key]) ? previous[key] : undefined 
                        });
                    }
               }
            }
            return result;
            
        }
        
        var subs = getSubs();

        var subsIn = subs.filter(function(sub) { return !sub.out });
        var subsOut = subs.filter(function(sub) { return !sub.in });
        var pairs = subs.filter(function(sub) { return sub.in && sub.out });

        var result = pairs.concat(
            subsIn.map(function(sub, index){
                var subOut = subsOut[index];
                return { in: sub.in, out: subOut ? subOut.out : undefined, positionChange: true };
            })
        );

        return (<div>
            {result.map(function(sub){
                return (
                    <div className='text-center gp-subs' key={sub.in + sub.out}>
                        <span className="gameplan-sub-in">{sub.in}</span> 
                        &nbsp;=&gt;&nbsp; 
                        <span className="gameplan-sub-out">{sub.out}</span>
                        {sub.positionChange? '*' : ''}
                    </div>
                )
            })}
         
        </div>);
    },

    renderGameTime: function () {
        var gameTime = [];
        var rows = this.state.rows;
        for (var index in rows) {
            var i = parseInt(index);
            if (i == rows.length - 1) {
                for (var j in rows[i]) {
                    gameTime.push(
                        {
                            player: rows[i][j],
                            minutes: 90 - rows[i].time
                        }
                    );
                }
            } else {
                for (var j in rows[i]) {
                    gameTime.push(
                        {
                            player: rows[i][j],
                            minutes: rows[i + 1].time - rows[i].time
                        }
                    );
                }
            }
        }

        var total = {};
        for (var k in gameTime) {
            var element = gameTime[k];
            if (isNaN(element.player)) {
                if (element.player in total) {
                    total[element.player] += element.minutes;
                } else {
                    total[element.player] = element.minutes;
                }
            }
        }

        function getPlayerTime (player, value) { 
            return (<div key={player.Id}>
                {player.Name}:&nbsp;<b>{value || 0}</b></div>
                ); 
            }
      
        var result = [];
        for (var player of this.props.players) {
            result.push(getPlayerTime(player, total[player.Name]));
        }
        return (<div>{result}</div>);
    },
    renderPublishButton: function () {
        if (this.props.iscoach == 'False') return '';
        if (this.state.isPublished || this.props.ispublished == 'True') {
            return (<div className='text-center'>
                                <div className='disabled btn btn-lg btn-success'><i className='fa fa-check-circle'></i> Publisert</div>
                            </div>);
        }
        return (<div className='text-center hidden-print'><button onClick={this.publish} className='btn btn-lg btn-success'><span className={this.state.isPublishing ? 'hidden' : ''}>Publiser bytteplan</span>
                    <i className={this.state.isPublishing ? 'fa fa-spinner fa-spin' : 'hidden'}></i></button></div>
                );
    }
});



