import axios from 'axios';
import { setAlert } from './alert';
import { REGISTER_SUCCESS, REGISTER_FAIL, USER_LOADED, AUTH_ERROR } from './types';
import setAuthToken from '../utils/setAuthToken';

// Load User
export const loadUser = () => async (dispatch) => {
	if (localStorage.token) {
		setAuthToken(localStorage.token);
	}

	const config = {
		headers: {
			Authorization: `Bearer ${localStorage.token}`
		}
	};

	try {
		const res = await axios.get('/api/users/currentUser', config);

		dispatch({
			type: USER_LOADED,
			payload: res.data
		});
	} catch (err) {
		dispatch({
			type: AUTH_ERROR
		});
	}
};

// Register User
export const register = ({ username, email, password }) => async (dispatch) => {
	const config = {
		headers: {
			'Content-Type': 'application/json'
		}
	};

	const body = JSON.stringify({ username, email, password });

	try {
		const res = await axios.post('/api/auth/register', body, config);

		dispatch({
			type: REGISTER_SUCCESS,
			payload: res.data
		});
	} catch (err) {
		const errors = err.response.data;

		console.log(errors);

		if (errors) {
			errors.forEach((error) => dispatch(setAlert(error.description, 'danger ')));
		}

		// if (errors.Email) {
		// 	errors.Email.forEach((error) => dispatch(setAlert(error, 'danger')));
		// }

		// if (errors.Username) {
		// 	errors.Username.forEach((error) => dispatch(setAlert(error, 'danger')));
		// }

		// if (errors.Password) {
		// 	errors.Password.forEach((error) => dispatch(setAlert(error, 'danger')));
		// }

		dispatch({
			type: REGISTER_FAIL
		});
	}
};
