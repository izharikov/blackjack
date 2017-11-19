import React, { Component } from 'react'
import styles from './scss/index';

export class Modal extends Component {
    constructor(props) {
        super(props);
        this.modalCloseBackGround = this.modalCloseBackGround.bind(this);
    }

    modalCloseBackGround(event) {
        let { isOpen, onModalClose = _ => { } } = this.props;
        if (event.target == event.currentTarget) {
            onModalClose();
        }
    }

    render() {
        let { isOpen, onModalClose = _ => { } } = this.props;
        let displayStyle = isOpen ? 'block' : 'none';
        return <div className={`${styles.modal_wrapper}
                ${isOpen ? styles.open : styles.close}`}
            onClick={this.modalCloseBackGround}>
            <div className={`${styles.modal} ${styles.modal_animate}`}>
                {this.props.children}
            </div>
        </div >

    }
}

export class ModalHeader extends Component {
    render() {
        let { title, onModalClose = () => { } } = this.props;
        return (
            <div className={styles.modal_header}>
                <div className={styles.modal_title}>{title}</div>
                <button className={styles.modal_close} onClick={onModalClose}>×</button>
            </div>
        )
    }
}

export class ModalContent extends Component {
    render() {
        return <div className={styles.modal_content}>
            {this.props.children}
        </div>
    }
}

export class ModalFooter extends Component {
    render() {
        return <div className={styles.modal_footer}>
            {this.props.children}
        </div>
    }
}