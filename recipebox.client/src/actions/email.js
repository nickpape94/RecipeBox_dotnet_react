import axios from 'axios';
import { setAlert } from './alert';
import { PASSWORD_RESET_EMAIL_SUCCESS, PASSWORD_RESET_EMAIL_FAIL } from './types';

// Send password reset link
export const passwordReset = (email) => async (dispatch) => {
	const config = {
		headers: {
			'Content-Type': 'application/json'
		}
	};

	const body = JSON.stringify(email);

	try {
		const res = await axios.post('/api/auth/forgetPassword', body, config);

		dispatch({
			type: PASSWORD_RESET_EMAIL_SUCCESS,
			payload: res.data
		});

		dispatch(
			setAlert('An email containing reset instructions has been delivered to your address. ', 'success'),
			'success'
		);
	} catch (err) {
		dispatch({
			type: PASSWORD_RESET_EMAIL_FAIL,
			payload: { msg: err.response.statusText, status: err.response.status }
		});

		dispatch(setAlert('No account with this information found.', 'danger'), 'danger');
	}
};
