import {
	REGISTER_SUCCESS,
	REGISTER_FAIL,
	AUTH_ERROR,
	USER_LOADED,
	LOGIN_FAIL,
	LOGOUT,
	LOGIN_SUCCESS,
	PASSWORD_RESET_SUCCESS,
	PASSWORD_RESET_FAIL,
	USER_PHOTO_UPLOAD_SUCCESS,
	USER_PHOTO_DELETED,
	USER_PHOTO_UPLOAD_FAIL,
	ADD_OR_UPDATE_ABOUT_SECTION,
	ABOUT_SECTION_ERROR
} from '../actions/types';

const initialState = {
	token: localStorage.getItem('token'),
	isAuthenticated: null,
	loading: true,
	user: null,
	error: null
};

export default function(state = initialState, action) {
	const { type, payload } = action;

	switch (type) {
		case USER_LOADED:
			return {
				...state,
				isAuthenticated: true,
				loading: false,
				user: payload
			};
		case PASSWORD_RESET_SUCCESS:
		case REGISTER_SUCCESS:
		case LOGIN_SUCCESS:
			localStorage.setItem('token', payload.token);
			return {
				...state,
				...payload,
				isAuthenticated: true,
				loading: false
			};
		case USER_PHOTO_UPLOAD_SUCCESS:
			return {
				...state,
				loading: false,
				isAuthenticated: true,
				user: {
					...state.user,
					userPhotos: [ payload ]
				}
			};
		case ADD_OR_UPDATE_ABOUT_SECTION:
			return {
				...state,
				loading: false,
				user: {
					...state.user,
					about: payload
				},
				error: null
			};
		case USER_PHOTO_DELETED:
			return {
				...state,
				loading: false,
				isAuthenticated: true,
				user: {
					...state.user,
					userPhotos: []
				}
			};
		case ABOUT_SECTION_ERROR:
			return {
				...state,
				loading: false,
				error: payload
			};
		case REGISTER_FAIL:
		case LOGIN_FAIL:
		case LOGOUT:
		case AUTH_ERROR:
		case PASSWORD_RESET_FAIL:
			localStorage.removeItem('token');
			return {
				...state,
				token: null,
				isAuthenticated: false,
				loading: false,
				user: null
			};
		default:
			return state;
	}
}
