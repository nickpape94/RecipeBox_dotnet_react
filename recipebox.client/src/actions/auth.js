import axios from 'axios';
import { setAlert } from './alert';
import {
	REGISTER_SUCCESS,
	REGISTER_FAIL,
	USER_LOADED,
	AUTH_ERROR,
	LOGIN_SUCCESS,
	LOGIN_FAIL,
	LOGOUT,
	PASSWORD_RESET_SUCCESS,
	PASSWORD_RESET_FAIL
} from './types';
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
		// console.log(err);

		dispatch({
			type: AUTH_ERROR,
			payload: { msg: err.response.statusText, status: err.response.status }
		});
	}
};

// Register User
export const register = ({ username, email, password, submitting }) => async (dispatch) => {
	const config = {
		headers: {
			'Content-Type': 'application/json'
		}
	};

	const body = JSON.stringify({ username, email, password });

	try {
		const res = await axios.post('/api/auth/register', body, config);

		// submitting(true);

		dispatch({
			type: REGISTER_SUCCESS,
			payload: res.data
		});

		dispatch(loadUser());
	} catch (err) {
		submitting(false);
		const errors = err.response.data;

		// console.log(errors);

		if (errors) {
			errors.forEach((error) => dispatch(setAlert(error.description, 'danger ')));
		}

		dispatch({
			type: REGISTER_FAIL
		});
	}
};

// Login
export const login = ({ email, password, location, history }) => async (dispatch) => {
	const config = {
		headers: {
			'Content-Type': 'application/json'
		}
	};

	const body = JSON.stringify({ email, password });

	try {
		const res = await axios.post('/api/auth/login', body, config);

		// console.log(res.data);

		dispatch({
			type: LOGIN_SUCCESS,
			payload: res.data
		});

		dispatch(loadUser());

		if (location !== undefined && location.state.fromPosts) {
			history.push('/submit-post');
		}

		if (location !== undefined && location.state.fromComments) {
			history.goBack();
		}
	} catch (err) {
		const errors = err.response.data;

		if (errors) {
			dispatch(setAlert('The username or password is incorrect.', 'danger'), 'danger');
		}

		dispatch({
			type: LOGIN_FAIL
		});
	}
};

// Reset password
export const resetPassword = ({ token, email, password }) => async (dispatch) => {
	const config = {
		headers: {
			'Content-Type': 'application/json'
		}
	};

	const body = JSON.stringify({ token, email, password });

	try {
		// if (newpassword !== confirmpassword) return res.statusText('Passwords do not match');

		const res = await axios.post('api/auth/resetPassword', body, config);

		dispatch({
			type: PASSWORD_RESET_SUCCESS,
			payload: res.data
		});

		dispatch(setAlert('Password has been updated.', 'success'));

		dispatch(loadUser());
	} catch (err) {
		const errors = err.response.data.errors;

		// console.log(errors);

		if (errors) {
			errors.forEach((error) => dispatch(setAlert(error, 'danger ')));
		}

		dispatch({
			type: PASSWORD_RESET_FAIL
		});
	}
};

// Logout
export const logout = () => (dispatch) => {
	dispatch({ type: LOGOUT });
};
