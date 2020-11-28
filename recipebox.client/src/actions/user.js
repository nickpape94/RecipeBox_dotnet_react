import axios from 'axios';
import {
	GET_USERS,
	GET_USER,
	USER_ERROR,
	RESET_PROFILE_PAGINATION_HEADERS,
	ADD_OR_UPDATE_ABOUT_SECTION,
	ABOUT_SECTION_ERROR
} from './types';

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

// Add or update about section on profile
export const addUpdateAboutSection = (userId, body, setAboutSubmit) => async (dispatch) => {
	const config = {
		headers: {
			'Content-Type': 'application/json',
			Authorization: `Bearer ${localStorage.token}`
		}
	};

	try {
		const res = await axios.post(`/api/users/${userId}/about`, JSON.stringify(body), config);

		dispatch({
			type: ADD_OR_UPDATE_ABOUT_SECTION,
			payload: res.data
		});

		setAboutSubmit(true);
	} catch (err) {
		// console.log(err.response.data);

		dispatch({
			type: ABOUT_SECTION_ERROR,
			payload: err.response.data
		});

		setAboutSubmit(false);
	}
};

export const resetProfilePagination = () => (dispatch) => {
	dispatch({ type: RESET_PROFILE_PAGINATION_HEADERS });
};
