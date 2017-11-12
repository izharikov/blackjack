const logger = store => next => action => {
  // console.log('dispatching', action, 'next : ', next, store.dispatch)
  let result = next(action)
  // console.log('next state', store.getState())
  return result
}

export default logger;