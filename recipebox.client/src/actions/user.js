import axios from 'axios';
import { GET_USERS, GET_USER, USER_ERROR, GET_USER_FAVOURITES, RESET_PROFILE_PAGINATION_HEADERS } from './types';

// Get users
export const getUsers = () => async (dispatch) => {
	try {
		const res = await axios.get('/api/users');

		dispatch({
			type: GET_USERS,
			payload: res.data
		});
	} catch (err) {
		dispatch({
			type: USER_ERROR,
			payload: { msg: err.response.statusText, status: err.response.status }
		});
	}
};

// Get user
export const getUser = (id, setUserLoading) => async (dispatch) => {
	try {
		setUserLoading(true);
		const res = await axios.get(`/api/users/${id}`);

		dispatch({
			type: GET_USER,
			payload: res.data
		});

		// Check url, if it ends in a number, it means we are on a users profile
		// In which case we want to reset profile pagination back to its initial state
		if (!isNaN(window.location.href.split('/')[window.location.href.split('/').length - 1])) {
			dispatch({
				type: RESET_PROFILE_PAGINATION_HEADERS
			});
		}

		setUserLoading(false);
	} catch (err) {
		dispatch({
			type: USER_ERROR,
			payload: { msg: err.response.statusText, status: err.response.status }
		});
	}
};
