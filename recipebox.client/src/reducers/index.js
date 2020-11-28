import { combineReducers } from 'redux';
import alert from './alert';
import auth from './auth';
import post from './post';
import email from './email';
import photo from './photo';
import user from './user';
import pagination from './pagination';
import favourite from './favourite';
import profilePagination from './profilePagination';

export default combineReducers({
	alert,
	auth,
	post,
	email,
	photo,
	user,
	pagination,
	profilePagination,
	favourite
});
