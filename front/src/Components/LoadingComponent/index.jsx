import React, { Component } from 'react'
import styles from './scss/loader-styles';

export class LoadingComponent extends Component {
  render() {
    return (
      <div className={styles.loader_wrapper}>
        <div className={styles.loader}/>
      </div>
    )
  }
}
