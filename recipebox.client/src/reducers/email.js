import { PASSWORD_RESET_EMAIL_FAIL, PASSWORD_RESET_EMAIL_SUCCESS } from '../actions/types';

const initialState = {
	loading: true
};

export default function(state = initialState, action) {
	const { type, payload } = action;

	switch (type) {
		case PASSWORD_RESET_EMAIL_SUCCESS:
			return {
				...state,
				...payload,
				loading: false
			};
		case PASSWORD_RESET_EMAIL_FAIL:
			return {
				...state,
				loading: false
			};
		default:
			return state;
	}
}
