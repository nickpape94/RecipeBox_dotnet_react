import axios from 'axios';
import { GET_USERS, GET_USER, USER_ERROR, GET_USER_FAVOURITES } from './types';

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
export const getUser = (id) => async (dispatch) => {
	try {
		const res = await axios.get(`/api/users/${id}`);
		const favourites = await axios.get(`/api/favourites/userId/${id}`);

		dispatch({
			type: GET_USER,
			payload: res.data
		});

		dispatch({
			type: GET_USER_FAVOURITES,
			payload: favourites.data
		});
	} catch (err) {
		dispatch({
			type: USER_ERROR,
			payload: { msg: err.response.statusText, status: err.response.status }
		});
	}
};
