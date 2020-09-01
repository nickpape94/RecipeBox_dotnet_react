import { combineReducers } from 'redux';
import alert from './alert';
import auth from './auth';
import post from './post';
import email from './email';

export default combineReducers({
	alert,
	auth,
	post,
	email
});
