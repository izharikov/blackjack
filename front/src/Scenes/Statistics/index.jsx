import React, { Component } from 'react'
import rest from '../../common/rest';
import { connect } from 'react-redux';
import { CALL_API } from 'redux-api-middleware';
import { PageWrapper } from '../../Components/PageWrapper/index';
import styles from './scss';

class Statistics extends Component {
    constructor(props) {
        super(props);
        // this.loadStatistics = props.loadStatistics;
    }

    componentWillMount() {
        this.props.loadStatistics();
    }

    renderGameResultTable = (gameResult) => {
        return <table className={`${styles.table} ${styles.table_centered}`}>
            <thead>
                <tr>
                    <th>Winner score</th>
                    <th>Is winner</th>
                </tr>
            </thead>
            <tbody>
                {gameResult.map(res => (
                    <tr key={res.gameId}>
                        <td>{res.winnerScore}</td>
                        <td>{res.isWinner ? '✓' : '×'}</td>
                    </tr>
                ))}
            </tbody>
        </table>;
    }

    render() {
        let { gameResult = [] } = this.props;
        return (
            <PageWrapper>
                <h1 className={styles.centered}>Game results</h1>
                {gameResult.length ?
                    <div className={styles.centered}>
                        {this.renderGameResultTable(gameResult)}
                    </div> : <div> There is no played games</div>}
            </PageWrapper>
        )
    }
}

const STATISTICS_REQUEST = "STATISTICS_REQUEST";
const STATISTICS_SUCCESS = "STATISTICS_SUCCESS";
const STATISTICS_FAILURE = "STATISTICS_FAILURE";

const actions = {
    statistics: () => ({
        [CALL_API]: {
            endpoint: rest.statistics,
            method: 'GET',
            headers: { 'Content-Type': 'application/json' },
            types: [STATISTICS_REQUEST, STATISTICS_SUCCESS, STATISTICS_FAILURE],
            credentials: 'same-origin'
        }
    })
}

export const statisticsReducer = (state = {}, action) => {
    switch (action.type) {
        case STATISTICS_SUCCESS:
            return action.payload;
        default:
            return state;
    }
};

const mapStateToProps = (state) => {
    return state.statistics;
}

const mapDispathToProps = (dispatch, ownProps) => {
    return {
        loadStatistics: () => dispatch(actions.statistics())
    }
}

export const StatisticsPage = connect(mapStateToProps, mapDispathToProps)(Statistics);