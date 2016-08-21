var React = require('react');

module.exports = React.createClass({
    getInitialState: function () {
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
            ]
        });
    },

    setPlayer: function (i, key, input) {
        var rows = this.state.rows;
        rows[i][key] = input.target.value;
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
    },

    render: function () {
        console.log(this.props)
        var that = this;
        return (
            <div className='gameplan'>
                <div className='mt-main'>
                    <div className='mt-container'>
                        {this.state.rows.map(function (lineup, i) {
                            return (<div key={i}>
                                <div className='text-center'>
                                    <input className='gp-time' onChange={that.setTime.bind(null, i)} placeholder='tid' value={lineup.time} />min
                                </div>
                                    <button className='pull-right' onClick={that.removeRow.bind(null, i)}><i className='fa fa-times'></i></button>
                                <br />
                                <br />
                                <div className='gp-row'>
                                    {that.renderDiff(i)}
                                </div>
                                <div className='gp-row'>
                                    <div className='gp-square'><input onChange={that.setPlayer.bind(null, i, 'lw')} value={lineup.lw} /></div>
                                    <div className='gp-square'></div>
                                    <div className='gp-square'><input onChange={that.setPlayer.bind(null, i, 's')} value={lineup.s} /></div>
                                    <div className='gp-square'></div>
                                    <div className='gp-square'><input onChange={that.setPlayer.bind(null, i, 'rw')} value={lineup.rw} /></div>
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
                                    <div className='gp-square'><input onChange={that.setPlayer.bind(null, i, 'lcm')} value={lineup.lcm} /></div>
                                    <div className='gp-square'></div>
                                    <div className='gp-square'><input onChange={that.setPlayer.bind(null, i, 'rcm')} value={lineup.rcm} /></div>
                                    <div className='gp-square'></div>
                                </div>
                                <div className='gp-row'>
                                    <div className='gp-square'></div>
                                    <div className='gp-square'></div>
                                    <div className='gp-square'><input onChange={that.setPlayer.bind(null, i, 'dm')} value={lineup.dm} /></div>
                                    <div className='gp-square'></div>
                                    <div className='gp-square'></div>
                                </div>
                                <div className='gp-row'>
                                    <div className='gp-square'><input onChange={that.setPlayer.bind(null, i, 'lb')} value={lineup.lb} /></div>
                                    <div className='gp-square'><input onChange={that.setPlayer.bind(null, i, 'lcb')} value={lineup.lcb} /></div>
                                    <div className='gp-square'></div>
                                    <div className='gp-square'><input onChange={that.setPlayer.bind(null, i, 'rcb')} value={lineup.rcb} /></div>
                                    <div className='gp-square'><input onChange={that.setPlayer.bind(null, i, 'rb')} value={lineup.rb} /></div>
                                </div>
                                <div className='gp-row'>
                                    <div className='gp-square'></div>
                                    <div className='gp-square'></div>
                                    <div className='gp-square'><input onChange={that.setPlayer.bind(null, i, 'gk')} value={lineup.gk} /></div>
                                    <div className='gp-square'></div>
                                    <div className='gp-square'></div>
                                </div>
                                <hr />
                            </div>);
                        })}
                        <div className='text-center'>
                            <button className='btn btn-primary' onClick={this.duplicateRow}><i className='fa fa-plus'></i></button>
                        </div>
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

        function getSub (key) {
            if (previous[key] != current[key]) return (<div className='text-center gp-subs' key={i + key}>{current[key]} {'=>'} {previous[key]} </div>);
            else return (<span key={i + key}></span>);
        }

        var subs = [];
        subs.push(getSub('lw'));
        subs.push(getSub('s'));
        subs.push(getSub('rw'));
        subs.push(getSub('lcm'));
        subs.push(getSub('rcm'));
        subs.push(getSub('dm'));
        subs.push(getSub('lb'));
        subs.push(getSub('lcb'));
        subs.push(getSub('rcb'));
        subs.push(getSub('rb'));
        subs.push(getSub('gk'));

        return (<div>
            {subs}
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
            if (isNaN(element.player)){
                if (element.player in total) {
                    total[element.player] += element.minutes;
                } else {
                    total[element.player] = element.minutes;
                }
            }

        }

        function getPlayerTime (key, value) { return (<div key={key}>{key}: <b> {value}</b></div>); }

        var result = [];
        for (var l in total) {
            result.push(getPlayerTime(l, total[l]));
        }
        return (<div>{result}</div>);
    }
});



